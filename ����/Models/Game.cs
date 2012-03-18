using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace 象棋.Models
{    
    public class Animation
    {
        public 棋 Start { get; set; }
        public 棋 End { get; set; }                
    }

    public class ClientState
    {                
        public string Result { get; set; }
        public List<棋> Pieces { get; set; }
        public Animation Animation { get; set; }
        public int Step { get; set; }
        public string WhosTurn { get; set; }
    }

    public class Game : ICloneable
    {
        public static Game Instance = new Game();
        public Game LastInstance = null;

        int _step;
        List<棋> _pieces;
        Animation _lastAnimation;

        public 棋 GetPiece(int left, int top)
        {
            return _pieces.Find(p => p.Left == left && p.Top == top);
        }

        public int Step
        {
            get { return _step; }
        }

        public Game()
        {
            _step = 0;
            _pieces = new List<棋>{ 
                new 黑车() { Left = 0, Top=0 },
                new 黑马() { Left = 1, Top=0 },
                new 黑象() { Left = 2, Top=0 },
                new 黑士() { Left = 3, Top=0 },
                new 黑将() { Left = 4, Top=0 },
                new 黑士() { Left = 5, Top=0 },
                new 黑象() { Left = 6, Top=0 },
                new 黑马() { Left = 7, Top=0 },
                new 黑车() { Left = 8, Top=0 },
                new 黑炮() { Left = 1, Top=2 },
                new 黑炮() { Left = 7, Top=2 },
                new 黑卒() { Left = 0, Top=3 },
                new 黑卒() { Left = 2, Top=3 },
                new 黑卒() { Left = 4, Top=3 },
                new 黑卒() { Left = 6, Top=3 },
                new 黑卒() { Left = 8, Top=3 },

                new 红车() { Left = 0, Top=9 },
                new 红马() { Left = 1, Top=9 },
                new 红相() { Left = 2, Top=9 },
                new 红仕() { Left = 3, Top=9 },
                new 红帅() { Left = 4, Top=9 },
                new 红仕() { Left = 5, Top=9 },
                new 红相() { Left = 6, Top=9 },
                new 红马() { Left = 7, Top=9 },
                new 红车() { Left = 8, Top=9 },
                new 红炮() { Left = 1, Top=7 },
                new 红炮() { Left = 7, Top=7 },
                new 红兵() { Left = 0, Top=6 },
                new 红兵() { Left = 2, Top=6 },
                new 红兵() { Left = 4, Top=6 },
                new 红兵() { Left = 6, Top=6 },
                new 红兵() { Left = 8, Top=6 },
            };
        }

        public ClientState GetGame(string role)
        {        
            return _ConvertToClient(_step, role, "ok", _pieces, _lastAnimation);                        
        }

        public ClientState Play(string role, int fromX, int fromY, int toX, int toY)
        {
            //normalize
            if (role != "red")
            {
                fromX = 8 - fromX;
                fromY = 9 - fromY;
                toX = 8 - toX;
                toY = 9 - toY;
            }

            //validate
            var validateResult = _IsValid(role, fromX, fromY, toX, toY);
            if (validateResult.Item1 == false)
            {
                var r = _ConvertToClient(_step, role, validateResult.Item2, _pieces, null);                
                return r;
            }

            var old_state = (Game)this.Clone();
            old_state.LastInstance = LastInstance;
            LastInstance = old_state;

            //create client state (animation)
            var fromPiece = GetPiece(fromX, fromY);
            var fromPieceCopy = (棋)fromPiece.Clone();            

            var toPiece = GetPiece(toX, toY);
            fromPiece.Left = toX;
            fromPiece.Top = toY;

            if (toPiece != null)
                _pieces.Remove(toPiece);
            else
                toPiece = (棋)fromPiece.Clone();

            fromPiece.Left = toX;
            fromPiece.Top = toY;

            this._lastAnimation = new Animation { Start = fromPieceCopy, End = toPiece };
            var rr = _ConvertToClient(++_step, role, "ok", _pieces, _lastAnimation);            

            return rr;
        }

        public void Revert(string role)
        {
            if (LastInstance == null || this._WhosTurn() != role)
                return;
            this._step = LastInstance._step;
            this._pieces = LastInstance._pieces;
            this._lastAnimation = null;
            LastInstance = LastInstance.LastInstance;
        }

        string _WhosTurn()
        {
            return _step % 2 == 0 ? "red" : "black";
        }

        Tuple<bool,string> _IsValid(string role, int fromX, int fromY, int toX, int toY)
        {
            var fromPiece = GetPiece(fromX, fromY);
            if (fromPiece == null)
                return new Tuple<bool, string>(false, "no piece");

            if (fromPiece.Role != _WhosTurn())
                return new Tuple<bool, string>(false, "wrong turn");

            var r = fromPiece.CheckTurn(this, toX, toY);
            if (r.Item1 == false)
                return r;

            return new Tuple<bool,string>(true, "ok");
        }
        
        ClientState _ConvertToClient(int step, string role, string result, List<棋> pieces, Animation animation)
        {
            var newPieces = new List<棋>();
            foreach (var p in this._pieces)
                newPieces.Add((棋)p.Clone());                      

            if (role != "red")
            {
                foreach (var p in newPieces)
                {
                    p.Left = 8 - p.Left;
                    p.Top = 9 - p.Top;
                }
                if (animation != null)
                {
                    if (animation.Start != null)
                    {
                        animation.Start.Left = 8 - animation.Start.Left;
                        animation.Start.Top = 9 - animation.Start.Top;
                    }
                    if (animation.End != null)
                    {
                        animation.End.Left = 8 - animation.End.Left;
                        animation.End.Top = 9 - animation.End.Top;
                    }
                }
            }
            
            return new ClientState { Step = step, Result = result, Pieces = newPieces, Animation = animation, WhosTurn = _WhosTurn() };
        }

        public object Clone()
        {
            var g = new Game();
            g._step = this._step;
            g._pieces = new List<棋>();
            foreach (var p in this._pieces)
                g._pieces.Add((棋)p.Clone());
            return g;
        }
    }
}