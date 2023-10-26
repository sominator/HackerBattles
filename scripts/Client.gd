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

#signal to request GameManager to render player deck
signal render_player_deck(player_deck)

#signal to request GameManager to render opponent deck
signal render_opponent_deck(opponent_Deck)

#signal to request GameManager to handle moved card
signal render_opponent_moved_card(id, position)

#log server message to console
func _on_server_message(message):
	print("Server Message received: " + message)
	
#log game message to console
func _on_game_message(message):
	print("Game Message received:")
	print(message.action)
	if (message.action == "shuffle_decks"):
		emit_signal("render_player_deck", message.data.playerDeck)
		emit_signal("render_opponent_deck", message.data.opponentDeck)
	elif (message.action == "card_moved"):
		emit_signal("render_opponent_moved_card", message.data.id, message.data.posX, message.data.posY)
		print(message.data)

#send message to server that player card has been moved, splitting the "position" into posX and posY coordinates because Colyseus seems to have an issue messaging a Vector2
func _on_player_card_moved(ID, position):
	var message = {"action": "card_moved", "data": {"id": ID, "posX": position.x, "posY": position.y}}
	room.send("game-message", message)
