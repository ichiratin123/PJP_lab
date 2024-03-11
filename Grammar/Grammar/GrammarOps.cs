using Grammar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3
{
    public class GrammarOps
    {
        public GrammarOps(IGrammar g)
        {
            this.g = g;
            compute_empty();
            compute_first();
            compute_follow();
            PrintFirstSets();
            PrintFollowSets();
        }


        public ISet<Nonterminal> EmptyNonterminals { get; } = new HashSet<Nonterminal>();
        public Dictionary<Symbol, HashSet<string>> FirstSets { get; } = new Dictionary<Symbol, HashSet<string>>();
        public Dictionary<Nonterminal, HashSet<string>> FollowSets { get; } = new Dictionary<Nonterminal, HashSet<string>>();

        private Terminal epsilon = new Terminal("{e}");


        private void compute_empty()
        {
            int count;
            do
            {
                count = EmptyNonterminals.Count();
                foreach (var rule in g.Rules)
                {
                    if (rule.RHS.All(x => x is Nonterminal && EmptyNonterminals.Contains(x)))
                    {
                        EmptyNonterminals.Add(rule.LHS);
                    }
                }
            } while (count != EmptyNonterminals.Count);

        }

        private IGrammar g;

        private void compute_first()
        {
            foreach (Terminal t in g.Terminals)
            {
                FirstSets[t] = new HashSet<string> { t.Name };
            }

            foreach (Nonterminal nt in g.Nonterminals)
            {
                FirstSets[nt] = new HashSet<string>();
            }

            bool addedNewElements;
            do
            {
                addedNewElements = false;

                foreach (Rule rule in g.Rules)
                {
                    var lhs = rule.LHS;
                    var firstSetLHS = FirstSets[lhs];

                    if (rule.RHS.Count == 0)
                    {
                        // If there is a production X -> ε, add ε to FIRST(X)
                        if (!firstSetLHS.Contains("{e}"))
                        {
                            firstSetLHS.Add("{e}");
                            addedNewElements = true;
                        }
                    }
                    else
                    {
                        bool previousSymbolsDeriveEmpty = true;
                        foreach (Symbol symbol in rule.RHS)
                        {
                            if (!previousSymbolsDeriveEmpty) break;

                            if (symbol is Terminal terminal)
                            {
                                // If it's a terminal, add it to the FIRST set and break
                                if (!firstSetLHS.Contains(terminal.Name))
                                {
                                    firstSetLHS.Add(terminal.Name);
                                    addedNewElements = true;
                                }
                                break;
                            }
                            else if (symbol is Nonterminal nonterminal)
                            {
                                // Add all elements of FIRST(Yi) to FIRST(X)
                                var firstSetSymbol = FirstSets[nonterminal];
                                foreach (string sym in firstSetSymbol)
                                {
                                    if (sym != "{e}" && !firstSetLHS.Contains(sym))
                                    {
                                        firstSetLHS.Add(sym);
                                        addedNewElements = true;
                                    }
                                }

                                // If Yi doesn't derive ε, then stop adding
                                if (!firstSetSymbol.Contains("{e}"))
                                {
                                    previousSymbolsDeriveEmpty = false;
                                }
                            }
                        }

                        // If all Yi derive ε, add ε to FIRST(X)
                        if (previousSymbolsDeriveEmpty && !firstSetLHS.Contains("{e}"))
                        {
                            firstSetLHS.Add("{e}");
                            addedNewElements = true;
                        }
                    }
                }
            } while (addedNewElements);

        }

        public void PrintFirstSets()
        {
            foreach (var nonterminal in g.Nonterminals)
            {
                foreach (var rule in nonterminal.Rules)
                {
                    // The representation of a production as [A:BC]
                    String production;

                    if (!rule.RHS.Any())  // If there are no symbols in RHS, it's an empty production
                    {
                        production = $"{nonterminal.Name}:{string.Join("", "{e}")}";
                    }
                    else
                    {
                        production = $"{nonterminal.Name}:{string.Join("", rule.RHS.Select(s => s.Name))}";
                    }

                    // Fetch or compute the FIRST set for the RHS of the rule
                    var firstSetForRHS = ComputeFirstSetForRHS(rule.RHS);
                    // Convert the FIRST set to a string for printing, handling the special case for epsilon
                    string firstSetString = string.Join(" ", firstSetForRHS.Select(s => s == "{e}" ? "{e}" : s));
                    // Print the formatted output
                    Console.WriteLine($"first[{production}] = {firstSetString}");
                }
            }
        }

        private HashSet<string> ComputeFirstSetForRHS(IList<Symbol> rhsSymbols)
        {
            var firstSet = new HashSet<string>();
            bool addedEpsilon = false;

            foreach (var symbol in rhsSymbols)
            {
                if (symbol is Terminal terminal)
                {
                    firstSet.Add(terminal.Name);
                    break;
                }
                else if (symbol is Nonterminal nonterminal)
                {
                    var firstOfSymbol = FirstSets[nonterminal];
                    // Exclude epsilon unless it's the last symbol or all following can derive epsilon
                    var firstWithoutEpsilon = firstOfSymbol.Where(sym => sym != "{e}");
                    firstSet.UnionWith(firstWithoutEpsilon);

                    // If this nonterminal cannot derive epsilon, stop here
                    if (!firstOfSymbol.Contains("{e}"))
                    {
                        break;
                    }
                    else if (!addedEpsilon && rhsSymbols.Last() == symbol)
                    {
                        firstSet.Add("{e}"); // Add epsilon if it's the last symbol
                        addedEpsilon = true;
                    }
                }
            }

            // Handling case where all symbols can derive epsilon
            if (!addedEpsilon && rhsSymbols.All(sym => sym is Nonterminal nt && FirstSets[nt].Contains("{e}")))
            {
                firstSet.Add("{e}");
            }

            return firstSet;
        }


        private void compute_follow()
        {
            // Initialize FOLLOW sets for each non-terminal with an empty set
            foreach (Nonterminal nt in g.Nonterminals)
            {
                FollowSets[nt] = new HashSet<string>();
            }

            // The FOLLOW set of the starting symbol always contains the end-of-input marker
            FollowSets[g.StartingNonterminal].Add("$");

            // Keep iterating until no new elements can be added to any FOLLOW set
            bool addedNewElements;
            do
            {
                addedNewElements = false;
                foreach (Rule rule in g.Rules)
                {
                    var followSetLHS = FollowSets[rule.LHS];

                    for (int i = 0; i < rule.RHS.Count; i++)
                    {
                        var symbol = rule.RHS[i];

                        if (symbol is Nonterminal nonterminal)
                        {
                            var followSetNonterminal = FollowSets[nonterminal];
                            var beforeCount = followSetNonterminal.Count;

                            // If it's the last symbol, add FOLLOW(LHS) to FOLLOW(nonterminal)
                            if (i == rule.RHS.Count - 1)
                            {
                                followSetNonterminal.UnionWith(followSetLHS);
                            }
                            else
                            {
                                // For the next symbol, add FIRST(nextSymbol) - {epsilon} to FOLLOW(nonterminal)
                                var nextSymbol = rule.RHS[i + 1];
                                var firstNextSymbol = FirstSets[nextSymbol].Where(f => f != "{e}");
                                followSetNonterminal.UnionWith(firstNextSymbol);

                                // If FIRST(nextSymbol) contains epsilon, add FOLLOW(LHS) to FOLLOW(nonterminal)
                                if (FirstSets[nextSymbol].Contains("{e}"))
                                {
                                    followSetNonterminal.UnionWith(followSetLHS);
                                }
                            }

                            if (followSetNonterminal.Count > beforeCount)
                            {
                                addedNewElements = true;
                            }
                        }
                    }
                }
            } while (addedNewElements);
        }
        public void PrintFollowSets()
        {
            foreach (var nonterminal in g.Nonterminals)
            {
                string followSetString = string.Join(" ", FollowSets[nonterminal]);
                Console.WriteLine($"follow[{nonterminal.Name}] = {followSetString}");
            }
        }
    }
}
