using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace 象棋.Models
{
    public interface IValidator
    {
        Tuple<bool,string> CheckTurn(Game game, int fromX, int fromY, int toX, int toY);
    }

    public abstract class 基本规则 : IValidator
    {
        Tuple<bool, string> IValidator.CheckTurn(Game game, int fromX, int fromY, int toX, int toY)
        {
            var fromPiece = game.GetPiece(fromX, fromY);
            if (fromPiece == null)
                return new Tuple<bool, string>(false, "no piece");

            if (toX < 0 || toX > 8 || toY < 0 || toY > 9)
                return new Tuple<bool, string>(false, "outside");

            if (fromX == toX && fromY == toY)
                return new Tuple<bool, string>(false, "same position");

            var toPiece = game.GetPiece(toX, toY);
            if (toPiece != null && fromPiece.Role == toPiece.Role)
                return new Tuple<bool, string>(false, "wrong target");

            var r = CheckSpecificRule(game, fromX, fromY, toX, toY);
            if(!r)
                return new Tuple<bool, string>(false, "invalid step");
            return new Tuple<bool, string>(true, "ok");
        }

        public abstract bool CheckSpecificRule(Game game, int fromX, int fromY, int toX, int toY);

        public static bool IsOnSelfSide(string role, int toX, int toY)
        {
            if (role == "red")
            {
                return toY >= 5 && toY < 10;
            }
            else
            {
                return toY >= 0 && toY < 5;
            }
        }
    }

    public class 车规则 : 基本规则
    {
        public override bool CheckSpecificRule(Game game, int fromX, int fromY, int toX, int toY)
        {
            if ((fromX == toX || fromY == toY) == false)
                return false;

            if (fromX == toX)
            {
                int step = fromY < toY ? 1 : -1;
                for (int i = fromY + step; i != toY; i += step)
                {
                    if (game.GetPiece(fromX, i) != null)
                        return false;
                }
            }
            else
            {
                int step = fromX < toX ? 1 : -1;
                for (int i = fromX + step; i != toX; i += step)
                {
                    if (game.GetPiece(i, fromY) != null)
                        return false;
                }
            }

            return true;
        }        
    }

    public class 炮规则 : 基本规则
    {
        public override bool CheckSpecificRule(Game game, int fromX, int fromY, int toX, int toY)
        {
            if ((fromX == toX || fromY == toY) == false)
                return false;
            int n = 0;
            if (fromX == toX)
            {
                int step = fromY < toY ? 1 : -1;                
                for (int i = fromY + step; i != toY + step; i += step)
                {                    
                    if (game.GetPiece(fromX, i) != null)
                        n++;                    
                }
            }
            else
            {
                int step = fromX < toX ? 1 : -1;                
                for (int i = fromX + step; i != toX + step; i += step)
                {
                    if (game.GetPiece(i, fromY) != null)
                        n++;
                }
            }

            return n == 0 || n == 2;
        }
    }

    public class 相规则 : 基本规则
    {
        public override bool CheckSpecificRule(Game game, int fromX, int fromY, int toX, int toY)
        {
            var fromPiece = game.GetPiece(fromX, fromY);
            if (基本规则.IsOnSelfSide(fromPiece.Role, toX, toY) == false)
                return false;

            if ((Math.Abs(fromX - toX) == 2 && Math.Abs(fromY - toY) == 2) == false)
                return false;

            if (game.GetPiece(fromX + (toX - fromX) / 2, fromY + (toY - fromY) / 2) != null)
                return false;

            return true;
        }
    }

    public class 马规则 : 基本规则
    {
        public override bool CheckSpecificRule(Game game, int fromX, int fromY, int toX, int toY)
        {
            var dx = toX - fromX;
            var dy = toY - fromY;
            var adx = Math.Abs(dx);
            var ady = Math.Abs(dy);

            if((adx ==2 && ady == 1 || adx ==1 && ady == 2) == false)
                return false;

            if (adx > ady)
            {
                if (game.GetPiece(fromX + dx / 2, fromY) != null)
                    return false;
            }
            else
            {
                if (game.GetPiece(fromX, fromY + dy / 2) != null)
                    return false;
            }

            return true;
        }
    }

    public class 士规则 : 基本规则
    {
        public override bool CheckSpecificRule(Game game, int fromX, int fromY, int toX, int toY)
        {
            var fromPiece = game.GetPiece(fromX, fromY);

            if (IsOnSelfSide(fromPiece.Role, toX, toY) == false)
                return false;

            if ((toX >= 3 && toX <= 5) == false)
                return false;

            if((toY >= 0 && toY <= 2 || toY >= 7 && toY <= 9) == false)
                return false;

            var dx = toX - fromX;
            var dy = toY - fromY;
            var adx = Math.Abs(dx);
            var ady = Math.Abs(dy);

            if ((adx == 1 && ady == 1) == false)
                return false;

            return true;
        }
    }

    public class 将规则 : 基本规则
    {
        public override bool CheckSpecificRule(Game game, int fromX, int fromY, int toX, int toY)
        {
            var fromPiece = game.GetPiece(fromX, fromY);

            if (IsOnSelfSide(fromPiece.Role, toX, toY) == false)
                return false;

            if ((toX >= 3 && toX <= 5) == false)
                return false;

            if ((toY >= 0 && toY <= 2 || toY >= 7 && toY <= 9) == false)
                return false;

            var dx = toX - fromX;
            var dy = toY - fromY;
            var adx = Math.Abs(dx);
            var ady = Math.Abs(dy);

            if (adx + ady != 1)
                return false;

            return true;
        }
    }

    public class 兵规则 : 基本规则
    {
        public override bool CheckSpecificRule(Game game, int fromX, int fromY, int toX, int toY)
        {
            var fromPiece = game.GetPiece(fromX, fromY);
            
            var dx = toX - fromX;
            var dy = toY - fromY;
            var adx = Math.Abs(dx);
            var ady = Math.Abs(dy);
            
            if ((adx == 1 && ady == 0 || adx == 0 && ady == 1) == false)
                return false;

            if (IsOnSelfSide(fromPiece.Role, toX, toY) && adx == 1)
                return false;

            if (fromPiece.Role == "red" && dy > 0)
                return false;

            if (fromPiece.Role != "red" && dy < 0)
                return false;                

            return true;
        }
    }
}