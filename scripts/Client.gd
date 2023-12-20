extends Node

const colyseus = preload("res://addons/godot_colyseus/lib/colyseus.gd")
var room: colyseus.Room
var roomId = null

#set up basic schema
class GameState extends colyseus.Schema:
	static func define_fields():
		var mySynchronizedProperty = "Hello world"
		return [
			colyseus.Field.new("mySynchronizedProperty", colyseus.STRING, mySynchronizedProperty),
		]

func _ready():
	#set up client
	var client = colyseus.Client.new("wss://hacker-battles-f19d332ad0f0.herokuapp.com/")
	var promise
	if (roomId == null):
		promise = client.create(GameState, "game")
	else:
		promise = client.join_by_id(GameState, roomId)
	yield(promise, "completed")
	if promise.get_state() == promise.State.Failed:
		print("Failed")
		return
	var room: colyseus.Room = promise.get_result()
	room.on_message("server-message").on(funcref(self, "_on_server_message"))
	room.on_message("game-message").on(funcref(self, "_on_game_message"))
	self.room = room

#signal to let UIManager know both clients have connected
signal clients_connected()

#signal to let UIManger know client has disconnected
signal client_disconnected()

#signal to request GameManager to render player deck
signal render_player_deck(player_deck)

#signal to request GameManager to render opponent deck
signal render_opponent_deck(opponent_Deck)

#signal to request GameManager to handle moved card
signal render_opponent_moved_card(id, position)

#signal to request GameManager to handle flipped card
signal render_opponent_flipped_card(id)

#signal to request UIManager to update room id
signal update_roomId(roomId)

#signal to request UIManager to update opponent BP
signal update_opponent_bp(value)

#signal to request UIManager to update opponent variables
signal update_opponent_variables(value)

#signal to request UIManager to update objective
signal update_objective(index)

#signal to request UIManager to update opponent specialization
signal update_opponent_specialization(index)

#log server message to console
func _on_server_message(message):
	print("Server Message received: " + message)
	
#log game message to console
func _on_game_message(message):
	print("Game Message received:")
	print(message.action)
	if (message.action == "clients_connected"):
		emit_signal("clients_connected")
	elif (message.action == "client_disconnected"):
		emit_signal("client_disconnected")
	elif (message.action == "shuffle_decks"):
		emit_signal("render_player_deck", message.data.playerDeck)
		emit_signal("render_opponent_deck", message.data.opponentDeck)
	elif (message.action == "card_moved"):
		emit_signal("render_opponent_moved_card", message.data.id, message.data.posX, message.data.posY)
	elif (message.action == "card_flipped"):
		emit_signal("render_opponent_flipped_card", message.data.id)
	elif (message.action == "player_deck_shuffled"):
		emit_signal("render_player_deck", message.data.playerDeck)
	elif (message.action == "opponent_deck_shuffled"):
		emit_signal("render_opponent_deck", message.data.opponentDeck)
	elif (message.action == "update_roomId"):
		emit_signal("update_roomId", message.data.roomId)	
	elif (message.action == "bp_changed"):
		emit_signal("update_opponent_bp", message.data.value)
	elif (message.action == "variables_changed"):
		emit_signal("update_opponent_variables", message.data.value)
	elif (message.action == "objective_changed"):
		emit_signal("update_objective", message.data.index)
	elif (message.action == "specialization_changed"):
		emit_signal("update_opponent_specialization", message.data.index)

#send message to server that player card has been moved, splitting the "position" into posX and posY coordinates because Colyseus seems to have an issue messaging a Vector2
func _on_player_card_moved(ID, position):
	var message = {"action": "card_moved", "data": {"id": ID, "posX": position.x, "posY": position.y}}
	room.send("game-message", message)
	
func _on__player_card_flipped(ID):
	var message = {"action": "card_flipped", "data": {"id": ID}}
	room.send("game-message", message)

#send message to server to request a new shuffled player deck and render it on the opponent side
func _on_player_deck_shuffled():
	var message = {"action": "deck_shuffled"}
	room.send("game-message", message)

#send message to server with change in player BP to render on opponent side
func _on_player_bp_changed(value):
	var message = {"action": "bp_changed", "data": {"value": value}}
	room.send("game-message", message)

#send message to server with change in player variables to render on opponent side
func _on_player_variables_changed(value):
	var message = {"action": "variables_changed", "data": {"value": value}}
	room.send("game-message", message)

#send message to server with change in objective to render on opponent side
func _on_objective_changed(index):
	var message = {"action": "objective_changed", "data": {"index": index}}
	room.send("game-message", message)

#send message to server with change in player specialization to render on opponent side
func _on_player_specialization_changed(index):
	var message = {"action": "specialization_changed", "data": {"index": index}}
	room.send("game-message", message)
