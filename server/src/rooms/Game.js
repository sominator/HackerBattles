import { Room } from "@colyseus/core";
import { GameState } from "./schema/GameState.js";
import shuffle from "shuffle-array";

export class Game extends Room {

    //suppoort only 2 clients connected
    maxClients = 2;

    //determine what should happen when a room is created
    onCreate(options) {

        //track number of players
        this.numberOfPlayers = 0;

        console.log("Game Room created!", options);
        console.log("Room ID: " + this.roomId);

        //set a custom state from a created schema
        this.setState(new GameState());

        //when a message is received of type "message," broadcast it with the type "server-message" to all clients
        this.onMessage("message", (client, message) => {
            console.log("Game Room received message from", client.sessionId, ":", message);
            this.broadcast("server-message", `(${client.sessionId} ${message}`);
        });


        //when a message is received of type "game-message," broadcast it with the type "game-message" to all clients except for the one that sent it
        this.onMessage("game-message", (client, message) => {
            this.broadcast("game-message", message, { except: client });
        });

        this.shuffleDeck = () => {
            return shuffle(["boolean", "defrag", "double", "echo", "firewall", "float", "glitch", "handshake", "host", "ping", "probe", "reInitialize", "splice", "turnkey"]);
        }

    }

    //determine what should happen when a client joins
    onJoin(client, options) {
        this.numberOfPlayers++;
        console.log(client.sessionId, "joined!");
        this.broadcast("server-message", `${client.sessionId} joined.`);
        if (this.numberOfPlayers === 2) {
            this.clients[0].send("game-message", { action: "deal_cards", data: this.shuffleDeck() });
            this.clients[1].send("game-message", { action: "deal_cards", data: this.shuffleDeck() });
        }
    }

    //determine what should happen when a client leaves
    onLeave(client, consented) {
        this.numberOfPlayers--;
        console.log(client.sessionId, "left!");
        this.broadcast("server-message", `${client.sessionId} left.`);
    }

    //determine what should happen when a room is closed
    onDispose() {
        console.log("room", this.roomId, "disposing...");
    }

}
