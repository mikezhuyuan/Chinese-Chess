using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace 象棋.Models
{
    public class 黑将 : 棋
    {
        public 黑将()
        {
            validator = new 将规则();
        }
    }

    public class 黑士 : 棋
    {
        public 黑士()
        {
            validator = new 士规则();
        }
    }

    public class 黑象 : 棋
    {
        public 黑象()
        {
            validator = new 相规则();
        }
    }

    public class 黑马 : 棋
    {
        public 黑马()
        {
            validator = new 马规则();
        }
    }

    public class 黑车 : 棋
    {
        public 黑车()
        {
            validator = new 车规则();
        }
    }

    public class 黑炮 : 棋
    {
        public 黑炮()
        {
            validator = new 炮规则();
        }
    }

    public class 黑卒 : 棋
    {        
        public 黑卒()
        {
            validator = new 兵规则();
        }
    }

    public class 红帅 : 棋
    {
        public 红帅()
        {
            validator = new 将规则();
        }
    }

    public class 红仕 : 棋
    {
        public 红仕()
        {
            validator = new 士规则();
        }
    }

    public class 红相 : 棋
    {
        public 红相()
        {
            validator = new 相规则();
        }
    }

    public class 红马 : 棋
    {
        public 红马()
        {
            validator = new 马规则();
        }
    }

    public class 红车 : 棋
    {
        public 红车()
        {
            validator = new 车规则();
        }
    }

    public class 红炮 : 棋
    {
        public 红炮()
        {
            validator = new 炮规则();
        }
    }

    public class 红兵 : 棋
    {
        public 红兵()
        {
            validator = new 兵规则();
        }
    }

    public abstract class 棋 : ICloneable
    {
        protected IValidator validator;

        public Tuple<bool, string> CheckTurn(Game game, int toX, int toY)
        {
            return validator.CheckTurn(game, this.Left, this.Top, toX, toY);
        }

        public string Type
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public string Role
        {
            get
            {
                return this.GetType().Name.StartsWith("红") ? "red" : "black";
            }
        }        

        int _left;
        public int Left
        {
            get
            {
                return _left;
            }
            set
            {
                if (value < 0 || value > 8)
                    throw new ArgumentException();
                _left = value;
            }
        }

        int _top;
        public int Top
        {
            get
            {
                return _top;
            }
            set
            {
                if (value < 0 || value > 9)
                    throw new ArgumentException();
                _top = value;
            }
        }

        public object Clone()
        {
            var piece = (棋)Activator.CreateInstance(this.GetType());
            piece.Left = this.Left;
            piece.Top = this.Top;

            return piece;
        }
    }
}