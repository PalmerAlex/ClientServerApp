"use strict";
// Enables 'strict mode' in javascript

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Disable the buttons until connection is established.
document.getElementById("sendButton").disabled = true;
document.getElementById("sendGifButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    // This method is called when a message is received from the server.
    console.log("Message received");

    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // Creates a new list item and adds it to the messagesList UL on index.cshtml
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.

    li.textContent = `User-${user} : ${message}`;
    // Assigns the list item content here
});

connection.on("ReceiveGif", function (user) {
    // This method is called when a gif is received from the server.
    console.log("Gif received");

    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // Creates a new list item and adds it to the messagesList UL on index.cshtml

    li.innerHTML = `<span>User-${user} : </span><img src="https://media.tenor.com/izF-verFvhkAAAAC/chillin-frogs.gif" alt="Chilling Frog" height="100px" width="auto">`;
    // Assigns the list item content here
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    document.getElementById("sendGifButton").disabled = false;
    // Enable the buttons once connection is established.

}).catch(function (err) {
    return console.error(err.toString());
    // Prints any error message to console.
});

document.getElementById("roomButton").addEventListener("click", function (event) {
    // This method is called when the room button is pressed.
    var room = document.getElementById("roomInput").value;
    
    connection.invoke("RoomConnect", room).catch(function (err) {
        // Calls the RoomConnect method in ChatHub.cs
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    // This method is called when the send button is pressed.
    var message = document.getElementById("messageInput").value;
    
    connection.invoke("SendMessage", message).catch(function (err) {
        // Calls the SendMessage method in ChatHub.cs
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("sendGifButton").addEventListener("click", function (event) {
    // This method is called when the send button is pressed.

    connection.invoke("SendGif").catch(function (err) {
        // Calls the SendMessage method in ChatHub.cs
        return console.error(err.toString());
    });
    event.preventDefault();
});