namespace Ex02
{
    using System;

    internal class GameBoard
    {
        public const int k_MinSizeColumn = 4;
        public const int k_MinSizeRow = 4;

        public const int k_MaxSizeColumn = 6;
        public const int k_MaxSizeRow = 6;

        public static readonly Index sr_PassIndex = new Index(' ', -1);

        private readonly int r_SizeColumn;
        private readonly int r_SizeRow;

        private readonly Random r_RandomGenerator = new Random();

        private readonly Cell?[,] r_Board;

        private int m_ReaveledCellCount;
        private bool stoped;

        public GameBoard(int i_SizeColumn, int i_SizeRow)
        {
            this.r_SizeColumn = i_SizeColumn;
            this.r_SizeRow = i_SizeRow;
            this.m_ReaveledCellCount = 0;
            this.r_Board = new Cell?[i_SizeRow, i_SizeColumn];
            this.stoped = false;
            this.fillBoard();
        }

        public static bool IsValidRowAmount(int i_SizeRow)
        {
            return i_SizeRow <= k_MaxSizeRow && i_SizeRow >= k_MinSizeRow && i_SizeRow % 2 == 0;
        }

        public static bool IsValidColumnAmount(int i_SizeColumn)
        {
            return i_SizeColumn <= k_MaxSizeColumn && i_SizeColumn >= k_MinSizeColumn && i_SizeColumn % 2 == 0;
        }

        public bool IsValidIndexToShow(Index i_ToCheck)
        {
            return this.IsValidIndex(i_ToCheck) && this.r_Board[i_ToCheck.GetRow(), i_ToCheck.GetColumn()].GetValueOrDefault().GetIsHidden();
        }

        public bool AttemptSolve(Index i_IndexOne, Index i_IndexTwo)
        {
            if (this.IsValidIndexToShow(i_IndexOne) && this.IsValidIndexToShow(i_IndexTwo))
            {
                if (i_IndexOne != i_IndexTwo && this.r_Board[i_IndexOne.GetRow(), i_IndexOne.GetColumn()] == this.r_Board[i_IndexTwo.GetRow(), i_IndexTwo.GetColumn()])
                {
                    Cell tempToChangeA = this.r_Board[i_IndexOne.GetRow(), i_IndexOne.GetColumn()].GetValueOrDefault();
                    tempToChangeA.Reveal();
                    this.r_Board[i_IndexOne.GetRow(), i_IndexOne.GetColumn()] = tempToChangeA;

                    Cell tempToChangeB = this.r_Board[i_IndexTwo.GetRow(), i_IndexTwo.GetColumn()].GetValueOrDefault();
                    tempToChangeB.Reveal();
                    this.r_Board[i_IndexTwo.GetRow(), i_IndexTwo.GetColumn()] = tempToChangeB;

                    this.m_ReaveledCellCount += 2;
                    Console.WriteLine(this.r_Board[i_IndexTwo.GetRow(), i_IndexTwo.GetColumn()].GetValueOrDefault().GetIsHidden());
                    return true;
                }
            }

            return false;
        }

        public void ShowAll()
        {
            this.showUnwapred(null, null);
        }

        public void ShowRevealOne(Index i_IndexOne)
        {
            this.showUnwapred(i_IndexOne, null);
        }

        public void ShowRevealTwo(Index i_IndexOne, Index i_IndexTwo)
        {
            this.showUnwapred(i_IndexOne, i_IndexTwo);
        }

        public bool IsValidIndex(Index i_ToCheck)
        {
            return i_ToCheck.GetColumn() < this.r_SizeColumn && i_ToCheck.GetColumn() >= 0 && i_ToCheck.GetRow() < this.r_SizeRow && i_ToCheck.GetRow() >= 0;
        }

        public bool IsGameOver()
        {
            return this.m_ReaveledCellCount == this.r_SizeColumn * this.r_SizeRow;
        }

        public char GetValueAtIndex(Index i_GetFromIndex)
        {
            return this.r_Board[i_GetFromIndex.GetRow(), i_GetFromIndex.GetColumn()].GetValueOrDefault().GetValue();
        }

        public void Stop()
        {
            this.stoped = true;
        }

        public int GetSizeRow()
        {
            return this.r_SizeRow;
        }

        public int GetSizeColumn()
        {
            return this.r_SizeColumn;
        }

        private void showUnwapred(Index? i_IndexOne, Index? i_IndexTwo)
        {
            if (!this.stoped)
            {
                for (int row = -1; row < this.r_SizeRow; row++)
                {
                    if(row >= 0)
                    {
                        Console.Write(row);
                    }
                    else
                    {
                        Console.Write("  ");
                    }

                    for (int column = 0; column < this.r_SizeColumn; column++)
                    {
                        if (row == -1)
                        {
                            Console.Write("| {0}", Convert.ToChar(65 + column));
                        }
                        else
                        {
                            Console.Write("| ");
                            if ((i_IndexOne.HasValue && i_IndexOne.GetValueOrDefault().IsIndex(column, row))
                                || (i_IndexTwo.HasValue && i_IndexTwo.GetValueOrDefault().IsIndex(column, row)))
                            {
                                Console.Write(this.r_Board[row, column].GetValueOrDefault().GetValue());
                            }
                            else
                            {
                                Console.Write(this.r_Board[row, column].GetValueOrDefault());
                            }
                        }
                        Console.Write(" ");

                    }

                    Console.WriteLine(" |");

                    for (int column = 0; column < this.r_SizeColumn; column++)
                    {
                        Console.Write("====");
                    }

                    Console.WriteLine("===");
                }
            }
        }

        private void fillBoard()
        {
            for (int i = 0; i < this.r_SizeColumn * this.r_SizeRow / 2; i++)
            {
                char randomChar = Convert.ToChar(this.r_RandomGenerator.Next(65, 91));

                for (int j = 0; j < 2; j++)
                {
                    Index indexXy = this.getNullIndex();
                    this.r_Board[indexXy.GetRow(), indexXy.GetColumn()] = new Cell(randomChar);
                }
            }
        }

        private Index getNullIndex()
        {
            Index resultIndex = this.getRandomIndex();

            while (this.isBoardIndexNull(resultIndex))
            {
                resultIndex = this.getRandomIndex();
            }

            return resultIndex;
        }

        private bool isBoardIndexNull(Index i_ToCheck)
        {
            return this.r_Board[i_ToCheck.GetRow(), i_ToCheck.GetColumn()].HasValue;
        }

        private Index getRandomIndex()
        {
            int indexX = this.r_RandomGenerator.Next(0, this.r_SizeColumn);
            int indexY = this.r_RandomGenerator.Next(0, this.r_SizeRow);

            return new Index(indexX, indexY);
        }

        private struct Cell
        {
            private readonly char r_Value;
            private bool m_IsHidden;

            public Cell(char i_Value)
            {
                this.r_Value = i_Value;
                this.m_IsHidden = true;
            }

            public static bool operator ==(Cell i_One, Cell i_Two)
            {
                return (i_One.GetValue() == i_Two.GetValue()) && (i_One.GetIsHidden() == i_Two.GetIsHidden());
            }

            public static bool operator !=(Cell i_One, Cell i_Two)
            {
                return !(i_One == i_Two);
            }

            public override bool Equals(object i_OtherObject)
            {
                bool equalsResult = false;

                if (i_OtherObject is Cell)
                {
                    Cell cellOther = (Cell)i_OtherObject;
                    equalsResult = this.GetValue() == cellOther.GetValue() && this.GetIsHidden() == cellOther.GetIsHidden();
                }

                return equalsResult;
            }

            public char GetValue()
            {
                return this.r_Value;
            }

            public bool GetIsHidden()
            {
                return this.m_IsHidden;
            }

            public override string ToString()
            {
                return this.m_IsHidden ? " " : this.r_Value.ToString();
            }

            public void Reveal()
            {
                this.m_IsHidden = false;
            }
        }
    }
}
