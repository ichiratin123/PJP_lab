using System.Collections.Generic;
using System.Linq;

namespace Grammar
{

	public class Rule
	{

		public Rule(Nonterminal lhs)
		{
			this.LHS = lhs;
		}

		public Nonterminal LHS { get; init; }

		public IList<Symbol> RHS { get; } = new List<Symbol>();
        public override string ToString()
        {
            string lhsString = LHS.ToString();

            // Check if RHS is empty, and use "{e}" to represent an empty string
            string rhsString = RHS.Count == 0 ? "{e}" : string.Join(" ", RHS.Select(symbol => symbol.ToString()));

            return $"{lhsString} -> {rhsString}";
        }



    }
}