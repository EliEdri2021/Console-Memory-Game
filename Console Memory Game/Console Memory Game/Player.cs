namespace Ex02
{
    using System;
    using System.Collections.Generic;

    internal class Player
    {
        private readonly string r_Name;
        private readonly bool r_AiMode;
        private Ai m_AiInsance;
        private int m_Score;

        public Player(string i_Name, bool i_AiMode)
        {
            this.r_Name = i_Name;
            this.m_Score = 0;
            this.r_AiMode = i_AiMode;

            if (i_AiMode)
            {
                this.m_AiInsance = new Ai();
            }
        }

        public override string ToString()
        {
            string humanOrAi = this.r_AiMode ? "AI" : "Human";
            return string.Format("Player's Name:{0} ({1}){2}Score:{3}", this.r_Name, humanOrAi, System.Environment.NewLine, this.m_Score);
        }

        public string GetDisplayName()
        {
            string displayName;

            if (this.r_Name == null)
            {
                if (this.GetAiMode())
                {
                    displayName = "Ai";
                }
                else
                {
                    displayName = "Unknown";
                }
            }
            else
            {
                displayName = this.r_Name;
            }

            return displayName;
        }

        public bool GetAiMode()
        {
            return this.r_AiMode;
        }

        public void AddPoint()
        {
            this.m_Score++;
        }

        public ref Ai GetAi()
        {
            return ref this.m_AiInsance;
        }

        internal class Ai
        {
            private readonly System.Random r_RandomGenerator = new System.Random();

            private List<AiMemory> m_Memory;

            private Index? m_NextMoveBuffer;

            public Ai()
            {
                this.m_Memory = new List<AiMemory>();
            }

            public Index GenerateMove(ref GameBoard i_ActiveGameBoard)
            {
                this.m_NextMoveBuffer = null;
                Index? nextMove = null;

                for (int i = 0; i < this.m_Memory.Count; i++)
                {
                    for (int j = 0; j < this.m_Memory.Count; j++)
                    {
                        if (i != j && this.m_Memory[i].GetIndex() != this.m_Memory[j].GetIndex() && this.m_Memory[i].GetValue() == this.m_Memory[j].GetValue() && this.m_Memory[i].GetRelevant() == this.m_Memory[j].GetRelevant())
                        {
                            nextMove = this.m_Memory[i].GetIndex();
                            this.m_NextMoveBuffer = this.m_Memory[j].GetIndex();
                        }
                    }
                }

                if (!nextMove.HasValue)
                {
                    Index randomIndex = this.getRandomIndex(i_ActiveGameBoard);
                    while (!i_ActiveGameBoard.IsValidIndexToShow(randomIndex))
                    {
                        randomIndex = this.getRandomIndex(i_ActiveGameBoard);
                    }

                    this.m_NextMoveBuffer = null;
                    nextMove = randomIndex;
                }

                return nextMove.GetValueOrDefault();
            }

            public Index CompleteMove(ref GameBoard i_ActiveGameBoard, Index i_PreviousMove, char i_CharacterToMatch)
            {
                Index? nextMove = null;
                if (this.m_NextMoveBuffer.HasValue)
                {
                    nextMove = this.m_NextMoveBuffer.GetValueOrDefault();
                }
                else
                {
                    for (int i = 0; i < this.m_Memory.Count; i++)
                    {
                        if (i_CharacterToMatch.Equals(this.m_Memory[i].GetValue()) && i_PreviousMove != this.m_Memory[i].GetIndex() && this.m_Memory[i].GetRelevant() && m_Memory[i].GetIndex() != i_PreviousMove)
                        {
                            nextMove = this.m_Memory[i].GetIndex();
                        }
                    }
                }

                if (nextMove == null)
                {
                    nextMove = this.getRandomUnusedIndex(i_ActiveGameBoard);
                }

                return nextMove.GetValueOrDefault();
            }

            public void ConcludeRound(Index i_IndexA, char i_ValueAtIndexA, Index i_IndexB, char i_ValueAtIndexB, bool i_IsCombIrrevelant)
            {
                int? memIndexOfCellA = null;
                int? memIndexOfCellB = null;

                for (int i = 0; i < this.m_Memory.Count; i++)
                {
                    if (this.m_Memory[i].GetIndex().Equals(i_IndexA))
                    {
                        memIndexOfCellA = i;
                    }

                    if (this.m_Memory[i].GetIndex().Equals(i_IndexB))
                    {
                        memIndexOfCellB = i;
                    }
                }

                if (memIndexOfCellA.HasValue)
                {
                    if (i_IsCombIrrevelant)
                    {
                        AiMemory tempToChange = this.m_Memory[memIndexOfCellA.GetValueOrDefault()];
                        tempToChange.SetIrrelevant();
                        this.m_Memory[memIndexOfCellA.GetValueOrDefault()] = tempToChange;
                    }
                }
                else
                {
                    this.m_Memory.Add(new AiMemory(i_IndexA, i_ValueAtIndexA, !i_IsCombIrrevelant));
                }

                if (memIndexOfCellB.HasValue)
                {
                    if (i_IsCombIrrevelant)
                    {
                        AiMemory tempToChange = this.m_Memory[memIndexOfCellB.GetValueOrDefault()];
                        tempToChange.SetIrrelevant();
                        this.m_Memory[memIndexOfCellB.GetValueOrDefault()] = tempToChange;
                    }
                }
                else
                {
                    this.m_Memory.Add(new AiMemory(i_IndexB, i_ValueAtIndexB, !i_IsCombIrrevelant));
                }
            }

            private Index getRandomUnusedIndex(GameBoard i_ActiveGameBoard)
            {
                Index randomIndex = this.getRandomIndex(i_ActiveGameBoard);
                while (!i_ActiveGameBoard.IsValidIndexToShow(randomIndex))
                {
                    randomIndex = this.getRandomIndex(i_ActiveGameBoard);
                }

                return randomIndex;
            }

            private Index getRandomIndex(GameBoard i_ActiveGameBoard)
            {
                int row = this.r_RandomGenerator.Next(0, i_ActiveGameBoard.GetSizeRow() + 1);
                int column = this.r_RandomGenerator.Next(0, i_ActiveGameBoard.GetSizeColumn() + 1);

                return new Index(column, row);
            }

            private struct AiMemory
            {
                private readonly Index r_Index;
                private readonly char r_Value;
                private bool m_Relevant;

                public AiMemory(Index i_Index, char i_Value, bool i_Relevant)
                {
                    this.r_Index = i_Index;
                    this.r_Value = i_Value;
                    this.m_Relevant = i_Relevant;
                }

                public Index GetIndex()
                {
                    return this.r_Index;
                }

                public char GetValue()
                {
                    return this.r_Value;
                }

                public bool GetRelevant()
                {
                    return this.m_Relevant;
                }

                public void SetIrrelevant()
                {
                    this.m_Relevant = false;
                }
            }
        }
    }
}
