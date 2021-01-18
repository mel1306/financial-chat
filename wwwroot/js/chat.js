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
            //message_side = message_side === 'left' ? 'right' : 'left';
            message = new Message({
                text: text,
                message_side: 'left',
                userName: $('#currentUserName').val(),
                userId: $('#currentUserId').val(),
                //date: Date.now()
            });
            
            /*
            $.ajax({
                type: "POST",
                url: "/Home/Create",
                data: { text: text },
                dataType: "json",
                success: function (msg) {
                    console.log(msg);
                    connection.invoke("SendMessage", message).catch(function (err) {
                        return console.error(err.toString());
                    });
                    return $messages.animate({ scrollTop: $messages.prop('scrollHeight') }, 300);
                },
                error: function (req, status, error) {
                    console.log(error);
                }
            });
            */

            return $messages.animate({ scrollTop: $messages.prop('scrollHeight') }, 300);
        };
        renderMessage = function (message) {
            var $messages;
            $('.message_input').val('');
            $messages = $('.messages');
            //message_side = message_side === 'left' ? 'right' : 'left';
            //message_side = isCurrentUser ? 'left' : 'right';
            //message = new Message({
            //    text: text,
            //    message_side: message_side
            //});
            //var currentUser = $('#currentUserName').val();
            //if (!message.message_side)
            //    message.message_side = currentUser === message.userName ? 'left' : 'right';
            message.draw();
            return $messages.animate({ scrollTop: $messages.prop('scrollHeight') }, 300);
        };

        $('#rabbitMQ').click(function (e) {
            return sendMessageRabbitMQ(getMessageText());
        });

        $('.send_message').click(function (e) {
            return sendMessage(getMessageText());
        });

        $('.message_input').keyup(function (e) {
            if (e.which === 13) {
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

            //return $messages.animate({ scrollTop: $messages.prop('scrollHeight') }, 300);
        };

        sendMessageRabbitMQ = function (text) {
            var currentUser = $('#currentUserName').val();
            $.ajax({
                type: "POST",
                url: "/Home/RabbitMQ",
                dataType: "json",
                data: { text: text },
                success: function (result) {
                    
                },
                error: function (req, status, error) {
                    console.log(error);
                }
            });

            //return $messages.animate({ scrollTop: $messages.prop('scrollHeight') }, 300);
        };

        /*
        sendMessage('Hello Philip! :)');
        setTimeout(function () {
            return sendMessage('Hi Sandy! How are you?');
        }, 1000);
        return setTimeout(function () {
            return sendMessage('I\'m fine, thank you!');
        }, 2000);
        */

        getMessages();
    });
}.call(this));