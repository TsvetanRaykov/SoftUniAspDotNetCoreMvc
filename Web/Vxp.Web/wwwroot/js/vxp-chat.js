$(document).ready(() => {

    const btn = document.getElementById("vxp-chat-send-button");
    if (btn) {

        let connection = null;

        const setupConnection = () => {
            connection = new signalR.HubConnectionBuilder()
                .withUrl("/messaging")
                .build();

            connection.on("UpdateChatWindow", (data) => {
                const message = JSON.parse(data);
                const chatWindow = document.getElementById("vxp-chat-window");
                chatWindow.innerHTML += `<br />${message.CreatedOnString} <span class="text-success">${message.SenderName} > </span> ${message.Message}`;
                scrollToLatestMessage(chatWindow);
            });

            connection.on("LoadConversation", (data) => {
                const messages = JSON.parse(data);
                const chatWindow = document.getElementById("vxp-chat-window");

                for (let message of messages) {
                    chatWindow.innerHTML += `<br />${message.CreatedOnString} <span class="text-success">${message.SenderName} > </span> ${message.Message}`;
                }

                scrollToLatestMessage(chatWindow);

            });

            connection.start()
                .then(() => loadConversation())
                .catch(err => console.error(err.toString()));
        };

        setupConnection();


        btn.addEventListener("click",
            e => {
                e.preventDefault();
                const chatInput = $("#vxp-chat-input");
                const message = chatInput.val();
                chatInput.val("");
                const recipientId = document.getElementById("vxp-chat-recipient-id").value;
                const token = $('input[name="__RequestVerificationToken"]').val();

                fetch("/Chat",
                    {
                        method: "POST",
                        body: JSON.stringify({ recipientId, message }),
                        headers: {
                            'content-type': "application/json",
                            'RequestVerificationToken': token
                        }
                    })
                    .then(response => response.text())
                    .then(messageId => connection.invoke("TakeMessagesUpdate", messageId));

            });

        function loadConversation() {
            const recipientId = document.getElementById("vxp-chat-recipient-id").value;
            connection.invoke("LoadConversation", recipientId);
        }

        function scrollToLatestMessage(chatWindow) {
            $(chatWindow).stop().animate({
                scrollTop: $(chatWindow)[0].scrollHeight
            }, 800);
        }
    }
});