"use strict";
// Enables 'strict mode' in javascript

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    // This method is called when a message is received from the server.

    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // Creates a new list item and adds it to the messagesList UL on index.cshtml
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.

    li.textContent = `${user} says ${message}`;
    // Assigns the list item content here
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    // Enable the send button once connection is established.

}).catch(function (err) {
    return console.error(err.toString());
    // Prints any error message to console.
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    // This method is called when the send button is pressed.
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;

    connection.invoke("SendMessage", user, message).catch(function (err) {
        // Calls the SendMessage method in ChatHub.cs
        return console.error(err.toString());
    });
    event.preventDefault();
});