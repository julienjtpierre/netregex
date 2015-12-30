using NetRegex.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetRegex
{
    public class RegexExecution
    {
        #region Private Fields

        private static readonly Regex _namedGroupPattern = new Regex(@"\(\?\<([^!=]+?)\>");

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the execution start time.
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Gets the total execution duration.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Gets or sets the pattern string.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the replace string.
        /// </summary>
        public string Replace { get; set; }

        /// <summary>
        /// Gets or sets whether multiple strings are being matched.
        /// </summary>
        public bool MatchMultipleStrings { get; set; }

        /// <summary>
        /// Gets or sets the regex options.
        /// </summary>
        public RegexOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the text on which to perform the match.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets the replaced text, after execution has completed.
        /// </summary>
        public string ReplacedText { get; private set; }

        /// <summary>
        /// Gets the list of matches yielded from execution.
        /// </summary>
        public IEnumerable<Match> Matches { get; private set; }

        /// <summary>
        /// Gets the error message encountered, if any.
        /// </summary>
        public string ErrorMessage { get; set; }


        public IEnumerable<string> MatchNodes { get; private set; }


        //public IEnumerable<TreeNode> SplitNodes { get; set; }

        /// <summary>
        /// Gets the list of named groups extracted from the pattern.
        /// </summary>
        public IEnumerable<string> NamedGroups { get; private set; }

        /// <summary>
        /// Gets the analysis description obtained from Regex Workbench.
        /// </summary>
        public string AnalysisDescription { get; private set; }

        /// <summary>
        /// Gets or sets whether execution should yield all matches, even if the limit is exceeded.
        /// </summary>
        public bool IgnoreMaximumMatches { get; set; }

        /// <summary>
        /// Gets the list of groups yielded from all regex matches.
        /// </summary>
        public IEnumerable<Group> Groups
        {
            get
            {
                foreach (Match match in Matches)
                    foreach (Group group in match.Groups.Cast<Group>().Skip(1))
                        yield return group;
            }
        }

        /// <summary>
        /// Executes the regex.
        /// </summary>
        public void Execute()
        {
            StartTime = DateTime.Now;
            Matches = Enumerable.Empty<Match>();
            MatchNodes = Enumerable.Empty<string>();
            //SplitNodes = Enumerable.Empty<TreeNode>();
            NamedGroups = Enumerable.Empty<string>();

            if (!string.IsNullOrEmpty(Pattern) && !string.IsNullOrEmpty(Text))
            {
                try
                {
                    if (MatchMultipleStrings)
                        Options = Options | RegexOptions.Multiline;

                    Regex regex = new Regex(Pattern, Options);
                    Matches = regex.Matches(Text).Cast<Match>().ToArray();
                    NamedGroups = _namedGroupPattern.Matches(Pattern).Cast<Match>().Select(m => m.Groups[1].Value);

                    if (Replace != "")
                        ReplacedText = regex.Replace(Text, Replace ?? "");

                    MatchNodes = LoadMatchNodes();
                    //SplitNodes = LoadSplitNodes(regex.Split(Text));
                    AnalysisDescription = Analyze();
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }

                Duration = DateTime.Now - StartTime;
            }
        }

        #endregion

        #region Private Members

        private string Analyze()
        {
            RegexBuffer buffer = new RegexBuffer(Pattern) { RegexOptions = Options };

            try
            {
                return new RegexExpression(buffer).ToString(0);
            }
            catch (Exception ex)
            {
                return string.Format("An error occurred while analyzing the pattern: \"{0}\".", ex.Message);
            }
        }

        private IEnumerable<string> LoadMatchNodes()
        {
            List<string> nodes = new List<string>();
            List<string> namedGroups = NamedGroups.ToList();

            foreach (Match match in Matches)
            {
                List<Group> groupList = match.Groups.Cast<Group>().ToList();

                nodes.Add(match.Value);
                //nodes.Add(new TreeNode(string.Format("Match[{0}]: '{1}'", nodes.Count, match.Value.Truncate(100)), (
                //    Enumerable.Concat(
                //        from groupName in namedGroups
                //        let captureGroup = match.Groups[groupName]
                //        select new TreeNode(string.Format("Group<{0}>: '{1}'", groupName, captureGroup.Value)) { Tag = captureGroup },
                //        from matchGroup in groupList.Skip(1)
                //        select new TreeNode(string.Format("Group[{0}]: '{1}'", groupList.IndexOf(matchGroup), matchGroup.Value.Truncate(50))) { Tag = matchGroup }
                //    )
                //).ToArray())
                //{ Tag = match });
            }

            return nodes;
        }

        //private IEnumerable<TreeNode> LoadSplitNodes(string[] segments)
        //{
        //    List<TreeNode> nodes = new List<TreeNode>();
        //    int position = 0;

        //    foreach (string segment in segments)
        //    {
        //        int start = Text.IndexOf(segment, position);
        //        nodes.Add(new TreeNode(string.Format("Split[{0}]: '{1}'", nodes.Count, segment.Truncate(100))) { Tag = new StringSegment { Value = segment, Index = start, Length = segment.Length } });
        //        position = start + segment.Length;
        //    }

        //    return nodes;
        //}

        #endregion
    }
}
