using torrent.Script.Values;

namespace torrent.Script
{
    internal class ScriptUntil
    {
        public static bool Equeal(Value left, Value right)
        {
            left = left is RefrenceValue ? left.ToRefrence().Get() : left;
            right = right is RefrenceValue ? right.ToRefrence().Get() : right;

            /*In some script language (In the most in fact) This opreation will 
             * Convert the two type to the same and try to equel them.
             * In this script language wee will not say two types is equels. 
             * So "123" == 123 will give false. But toInt("123") == 123 will 
             * return true
             */
            if (left.Type() != right.Type())
                return false;

            if (left.Type() == "function")
                return left.ToFunction().Name() == right.ToFunction().Name();

            if (left.Type() == "string")
                return left.toString() == right.toString();

            if (left.Type() == "number")
                return left.ToNumber() == right.ToNumber();

            return left.ToPrimtiv() == right.ToPrimtiv();
        }

        public static Value GetValue(Value u)
        {
            if(u is RefrenceValue)
            {
                return GetValue(u.ToRefrence().Get());
            }
            return u;
        }
    }
}