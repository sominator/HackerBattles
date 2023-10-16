extends Node

const colyseus = preload("res://addons/godot_colyseus/lib/colyseus.gd")
var room: colyseus.Room

#set up basic schema
class GameState extends colyseus.Schema:
	static func define_fields():
		var mySynchronizedProperty = "Hello world"
		return [
			colyseus.Field.new("mySynchronizedProperty", colyseus.STRING, mySynchronizedProperty),
		]

func _ready():
	#set up client
	var client = colyseus.Client.new("ws://localhost:2567")
	var promise = client.join_or_create(GameState, "game")
	yield(promise, "completed")
	if promise.get_state() == promise.State.Failed:
		print("Failed")
		return
	var room: colyseus.Room = promise.get_result()
	room.on_message("server-message").on(funcref(self, "_on_server_message"))
	room.on_message("game-message").on(funcref(self, "_on_game_message"))
	self.room = room

#signal to request GameManager to shuffle and deal cards
signal deal_cards(data)

#signal to request GameManager to instance player cards
signal draw_cards

#signal to request GameManager to render opponent cards
signal render_cards

#signal to request GameManager to handle dropped card
signal dropped_card

#log server message to console
func _on_server_message(message):
	print("Server Message received: " + message)
	
#log game message to console
func _on_game_message(message):
	print("Game Message received:")
	print(message)
	if (message.action == "deal_cards"):
		emit_signal("deal_cards", message.data)
	if (message.action == "cards_drawn"):
		emit_signal("render_cards")
	elif (message.action == "card_dropped"):
		emit_signal("dropped_card")
				
#send message to server that cards have been drawn
func _on_cards_drawn():
	var message = {"action": "cards_drawn", "data": null}
	room.send("game-message", message)

#send message to server that card has been dropped
func _on_card_dropped():
	var message = {"action": "card_dropped", "data": null}
	room.send("game-message", message)
