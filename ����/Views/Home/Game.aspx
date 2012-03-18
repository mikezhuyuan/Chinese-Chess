<%@ Page Language="C#" MasterPageFile="~/Views/Home/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
    <style type="text/css">
        #board
        {
            width: 377px;
            height: 417px;
            background-image: url(../../Content/board.gif);
            background-position: 0 3px;
            position: relative;
            cursor: pointer;
        }
        
        #board .piece, .selected
        {
            cursor: pointer;
            width: 42px;
            height: 42px;
            position: absolute;
            background-image: url(../../Content/pieces.png);
            z-index: 999;
            padding: 0;
            margin: 0;
        }
        .selected
        {
            background-position: -294px 0;
        }
        
        .黑将
        {
            background-position: 0 0;
        }
        .黑士
        {
            background-position: -42px 0;
        }
        .黑象
        {
            background-position: -84px 0;
        }
        .黑马
        {
            background-position: -126px 0;
        }
        .黑车
        {
            background-position: -168px 0;
        }
        .黑炮
        {
            background-position: -210px 0;
        }
        .黑卒
        {
            background-position: -252px 0;
        }
        .红帅
        {
            background-position: 0 42px;
        }
        .红仕
        {
            background-position: -42px 42px;
        }
        .红相
        {
            background-position: -84px 42px;
        }
        .红马
        {
            background-position: -126px 42px;
        }
        .红车
        {
            background-position: -168px 42px;
        }
        .红炮
        {
            background-position: -210px 42px;
        }
        .红兵
        {
            background-position: -252px 42px;
        }
        
        #msg .red, #msg .black
        {
            display: none;
        }
        #msg.red .red
        {
            color: Red;
            display: block;
        }
        #msg.black .black
        {
            color: Black;
            display: block;
        }
    </style>
    <script src="../../Scripts/jquery-1.4.2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var current = null;
        var step = -1;
        var role = '<%=ViewData["Role"] %>';
        var Unit = 42;
        var game_state = null;
        $(function () {
            function createPiece(left, top, type, index, id) {
                left = Unit * left;
                top = Unit * top;
                return '<div style="left:' + left + 'px;top:' + top + 'px;" class="piece ' + type + '"' + (id ? (' id="' + id + '"') : "") + (typeof (index) != "undefined" ? (' index="' + index + '"') : "") + '></div>';
            }

            function update(state) {
                if (!state)
                    return;

                if (step == state.Step)
                    return;

                if (state.Result != "ok") {
                    return;
                }

                step = state.Step;
                current = null;
                game_state = state;
                $("#msg").attr("class", game_state.WhosTurn);
                if (role == game_state.WhosTurn)
                    $("#btnRevert").removeAttr("disabled");
                else
                    $("#btnRevert").attr("disabled", "disabled");

                var lst = [];
                for (var i = 0, len = state.Pieces.length; i < len; i++) {
                    var c = state.Pieces[i];
                    lst.push(createPiece(c.Left, c.Top, c.Type, i));
                }

                //animation doms
                if (state.Animation) {
                    var start = state.Animation.Start;
                    var end = state.Animation.End;
                    lst.push(createPiece(start.Left, start.Top, start.Type, -1, "piece_start"));
                    lst.push(createPiece(end.Left, end.Top, end.Type, -1, "piece_end"));
                }

                document.getElementById("board").innerHTML = lst.join('');

                //start animation
                if (state.Animation) {
                    var end = state.Animation.End;
                    $("#piece_start").animate({
                        left: end.Left * Unit,
                        top: end.Top * Unit
                    },
                        400,
                        function () {
                            $("#piece_start").remove();
                            $("#piece_end").remove();
                        }
                    );
                }

                //click piece
                $("#board > .piece").click(function (e) {
                    if (!game_state || game_state.WhosTurn != role)
                        return;

                    var self = $(this);
                    var index = parseInt(self.attr("index"));

                    if (game_state.Pieces[index].Role != role) {
                        return;
                    }

                    if (current) {
                        current.html("");
                    }
                    current = self.append('<div class="selected"></div>');
                    e.stopPropagation();
                });
            }

            //click board
            $("#board").click(function (e) {
                if (!game_state || game_state.WhosTurn != role)
                    return;

                if (!current)
                    return;

                var fromX = Math.floor(current.position().left / Unit);
                var fromY = Math.floor(current.position().top / Unit);
                var toX = Math.floor((e.pageX - this.offsetLeft) / Unit);
                var toY = Math.floor((e.pageY - this.offsetTop) / Unit);

                if (fromX == toX && fromY == toY)
                    return;

                play(fromX, fromY, toX, toY);
            });

            var lock = false;
            function play(fromX, fromY, toX, toY) {
                if (lock)
                    return;
                lock = true;
                $.ajax({
                    type: "POST",
                    url: "/Home/Play",
                    dataType: "JSON",
                    data: { role: role, step: step, fromX: fromX, fromY: fromY, toX: toX, toY: toY },
                    success: function (json) {
                        var state = $.parseJSON(json);
                        update(state);
                        lock = false;
                    }
                });
            }

            play();

            var lock2 = false;
            setInterval(function () {
                if (game_state && game_state.WhosTurn == role)
                    return;

                if (lock2)
                    return;
                lock2 = true;
                $.ajax({
                    type: "POST",
                    url: "/Home/Play",
                    dataType: "JSON",
                    data: { role: role, step: step },
                    success: function (json) {
                        var state = $.parseJSON(json);
                        update(state);
                        lock2 = false;
                    }
                });
            }, 1000);
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="msg">
        <h1 class="black">
            黑走</h1>
        <h1 class="red">
            红走</h1>
    </div>
    <div id="board">
    </div>
    <div>
        <% using (Html.BeginForm("Revert", "Home")){ %>                            
            <input id="btnRevert" type="submit" value="悔棋" disabled="disabled" />
            <input type="hidden" name="role" value='<%=ViewData["Role"] %>' />
        <% } %>
    </div>
</asp:Content>
