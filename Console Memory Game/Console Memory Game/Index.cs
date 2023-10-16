namespace Ex02
{
    public struct Index
    {
        private readonly int r_Column; // x
        private readonly int r_Row; // y

        public Index(int i_Column, int i_Row)
        {
            this.r_Column = i_Column;
            this.r_Row = i_Row;
        }

        public Index(char i_ColumnChar, int i_Row)
        {
            int charConvertedToIndex = System.Convert.ToInt16(i_ColumnChar);
            this.r_Column = charConvertedToIndex - 65;
            this.r_Row = i_Row;
        }

        public static bool operator ==(Index i_One, Index i_Two)
        {
            return (i_One.GetColumn() == i_Two.GetColumn()) && (i_One.GetRow() == i_Two.GetRow());
        }

        public static bool operator !=(Index i_One, Index i_Two)
        {
            return !(i_One == i_Two);
        }

        public override bool Equals(object i_OtherObject)
        {
            bool equalsResult = false;

            if (i_OtherObject is Index)
            {
                Index cellOther = (Index)i_OtherObject;
                equalsResult = this.GetColumn() == cellOther.GetColumn() && this.GetRow() == cellOther.GetRow();
            }

            return equalsResult;
        }

        public int GetColumn()
        {
            return this.r_Column;
        }

        public int GetRow()
        {
            return this.r_Row;
        }

        public bool IsIndex(int i_Column, int i_Row)
        {
            return this.r_Column == i_Column && this.r_Row == i_Row;
        }
    }
}