(function () {
    var Message;
    Message = function (arg) {
        this.text = arg.text, this.message_side = arg.message_side;
        this.userName = arg.userName;
        this.userId = arg.userId;
        //this.date = Date.now();
        this.draw = function (_this) {
            return function () {
                var $message;
                $message = $($('.message_template').clone().html());
                $message.addClass(_this.message_side).find('.text').html(_this.text);
                $('.messages').append($message);
                return setTimeout(function () {
                    return $message.addClass('appeared');
                }, 0);
            };
        }(this);
        return this;
    };
    $(function () {
        var getMessageText, sendMessage;
        var connection = new signalR.HubConnectionBuilder().withUrl("/Home/Index").build();

        connection.on("ReceiveMessage", function (msg) {
            var currentUser = $('#currentUserName').val();
            var message = new Message({
                text: msg.text,
                message_side: msg.userName === currentUser ? 'left' : 'right',
                userName: msg.userName,
                userId: msg.userID,
                date: msg.date
            });
            renderMessage(message);
            resizeChatWindow();
        });

        connection.start().then(function () {
        }).catch(function (err) {
            return console.error(err.toString());
        });

        getMessageText = function () {
            var $message_input;
            $message_input = $('.message_input');
            return $message_input.val();
        };
        sendMessage = function (text) {
            var $messages, message;
            if (text.trim() === '') {
                return;
            }
            $('.message_input').val('');
            $messages = $('.messages');
            message = new Message({
                text: text,
                message_side: 'left',
                userName: $('#currentUserName').val(),
                userId: $('#currentUserId').val(),
                //date: Date.now()
            });

            connection.invoke("SendMessage", message).catch(function (err) {
                return console.error(err.toString());
            });

            var jsonText = JSON.stringify(message);
            checkChatMessage(jsonText);
            //checkChatMessage(text);

            return $messages.animate({ scrollTop: $messages.prop('scrollHeight') }, 300);
        };
        renderMessage = function (message) {
            var $messages;
            $('.message_input').val('');
            $messages = $('.messages');
            message.draw();
            return $messages.animate({ scrollTop: $messages.prop('scrollHeight') }, 300);
        };

        $('.send_message').click(function (e) {
            resizeChatWindow();
            return sendMessage(getMessageText());
        });

        $('.message_input').keyup(function (e) {
            if (e.which === 13) {
                resizeChatWindow();
                return sendMessage(getMessageText());
            }
        });

        getMessages = function () {
            var currentUser = $('#currentUserName').val();
            $.ajax({
                type: "GET",
                url: "/Home/GetMessages",
                dataType: "json",
                success: function (result) {
                    var messages = JSON.parse(result);
                    messages.forEach(msg => {
                        var message = new Message({
                            text: msg.Text,
                            message_side: msg.UserName === currentUser ? 'left' : 'right',
                            userName: msg.UserName,
                            userId: msg.UserID,
                            //date: msg.Date
                        });
                        renderMessage(message);
                    });
                },
                error: function (req, status, error) {
                    console.log(error);
                }
            });
        };
        checkChatMessage = function (text) {
            $.ajax({
                type: "POST",
                url: "/Home/Bot",
                dataType: "json",
                data: { text: text },
                success: function (result) {
                    console.log(result);
                },
                error: function (req, status, error) {
                    console.log(error);
                }
            });
        };
        resizeChatWindow = function () {
            var count = $('.messages li').length;
            if (count > 50) {
                var items = $('.messages li').slice(0, count - 49).toArray();
                items.forEach(item => item.remove());
            }
        }
        getMessages();
    });
}.call(this));