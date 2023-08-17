extends Node

const colyseus = preload("res://addons/godot_colyseus/lib/colyseus.gd")
var room: colyseus.Room

class GameState extends colyseus.Schema:
	static func define_fields():
		var mySynchronizedProperty = "Hello world"
		return [
			colyseus.Field.new("mySynchronizedProperty", colyseus.STRING, mySynchronizedProperty),
		]

func _ready():
	var client = colyseus.Client.new("ws://localhost:2567")
	var promise = client.join_or_create(GameState, "game")
	yield(promise, "completed")
	if promise.get_state() == promise.State.Failed:
		print("Failed")
		return
	var room: colyseus.Room = promise.get_result()
	room.on_message("server-message").on(funcref(self, "_on_server_message"))
	room.on_message("game-message").on(funcref(self, "_on_game_message"))
	room.on_message("client-request").on(funcref(self, "_on_client_request"))
	self.room = room

#signal to request GameManager to render cards
signal draw_cards

#log server message to console
func _on_server_message(data):
	print(data)
	
#log game message to console
func _on_game_message(data):
	print(data)
	if (data == "draw"):
		print ("draw")
	elif (data == "drop"):
		print ("drop")
		
#log client request to console and draw cards
func _on_client_request(data):
	print (data)
	if (data.kind == "draw"):
		print ("draw")
		emit_signal("draw_cards")

#send request to server to draw cards on button down
func _on_button_down():
	room.send("client-request", "draw")
