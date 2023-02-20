using System.Text.RegularExpressions;

namespace ValidationMachine
{
    public class VMachine
    {
        public Dictionary<VM_Type, object[]> Validators = new();

        public Dictionary<(VM_Status, bool), Action> FeedbackActions = new();

        public List<VM_Status> Statuses = new();
        private Control Target { get; set; }

        public VMachine(Control control)
        {
            Target = control;
        }

        /// <summary>
        /// Adds a validator to the machine
        /// </summary>
        /// <param name="validator">Validator type to add</param>
        /// <param name="arguments">optional arguments (see "Validate" method for requirements)</param>
        public void Add(VM_Type validator, params Object[] arguments)
        {
                Validators ??= new Dictionary<VM_Type, Object[]>();
                Validators.Add(validator, arguments);
        }

        /// <summary>
        /// Test if target.Text is only letters
        /// </summary>
        private void AlphaTest()
        {
            Regex regex = new("^[a-zA-Z]*$");
            if (!regex.IsMatch(Target.Text))
            {
                Statuses.Add(VM_Status.Not_Alpha);
            }
        }

        /// <summary>
        /// Test if target.Text is only numbers
        /// </summary>
        private void NumTest()
        {
            Regex regex = new("^[0-9]*$");
            if (!regex.IsMatch(Target.Text))
            {
                Statuses.Add(VM_Status.Not_Num);
            }
        }

        /// <summary>
        /// Test if target.Text is only letters and numbers
        /// </summary>
        private void AlphaNumTest()
        {
            Regex regex = new("^[a-zA-Z0-9]*$");
            if (!regex.IsMatch(Target.Text))
            {
                Statuses.Add(VM_Status.Not_Alpha_Num);
            }
        }

        /// <summary>
        /// Tests if target.Text is equal to supplied string
        /// </summary>
        /// <param name="test">string to compare</param>
        private void EqualTest(string test)
        {
            if(Target.Text != test)
            {
                Statuses.Add(VM_Status.Not_Equal);
            }
        }

        /// <summary>
        /// Tests if target.Text contains a specified string
        /// </summary>
        /// <param name="test">string to check for</param>
        private void ContainsTest(string test)
        {
            if (!Target.Text.Contains(test))
            {
                Statuses.Add(VM_Status.Not_Contains);
            }
        }

        /// <summary>
        /// Tests if target.Text contains at least 1 special character
        /// </summary>
        private void SpecCharTest()
        {
            Regex regex = new (@"[!@#$%^&*()_+\-=[\]{};':""\\|,.<>/?]");
            if (!regex.IsMatch(Target.Text))
            {
                Statuses.Add(VM_Status.No_Spec_Char);
            }
        }

        /// <summary>
        /// Tests if target.Text meets a minimum length
        /// </summary>
        /// <param name="min">minimum character length</param>
        private void MinLenTest(int min)
        {
            if(Target.Text.Length < min)
            {
                Statuses.Add(VM_Status.Too_Short);
            }
        }

        /// <summary>
        /// Tests if target.Text is at or below a maximum length
        /// </summary>
        /// <param name="max">maximum character length</param>
        private void MaxLenTest(int max)
        {
            if (Target.Text.Length > max)
            {
                Statuses.Add(VM_Status.Too_Long);
            }
        }
        
        /// <summary>
        /// Tests if target.Text contains any characters specified as invalid (or defaults to special characters)
        /// </summary>
        /// <param name="chars">optional list of characters to check for</param>
        private void InvCharTest(List<char>? chars = null)
        {
            if(chars == null || chars.Count == 0)
            {
                Regex regex = new(@"[!@#$%^&*()_+\-=[\]{};':""\\|,.<>/?]");
                if (regex.IsMatch(Target.Text))
                {
                    Statuses.Add(VM_Status.Invalid_Char);
                }
            }
            else
            {
                foreach(Char c in chars)
                {
                    if (Target.Text.Contains(c))
                    {
                        Statuses.Add(VM_Status.Invalid_Char);
                    }
                }
            }
            
        }

        /// <summary>
        /// Tests if target.Text matches a regular expression
        /// </summary>
        /// <param name="regex">Regex to check against</param>
        private void RegExTest(Regex regex)
        {
            if (!regex.IsMatch(Target.Text))
            {
                Statuses.Add(VM_Status.RegEx_Fail);
            }
        }

        /// <summary>
        /// Tests if a specified function returns true
        /// </summary>
        /// <param name="func">method or lambda expression to test</param>
        private void FuncTest(Func<bool> func)
        {
            if (!func())
            {
                Statuses.Add(VM_Status.Func_Fail);
            }
        }

        /// <summary>
        /// Tests if a specified function returns true (override with a function that takes a string parameter)
        /// </summary>
        /// <param name="func">method or lambda expression to test</param>
        private void FuncTest(Func<string, bool> func)
        {
            if (!func(Target.Text))
            {
                Statuses.Add(VM_Status.Func_Fail);
            }
        }

        /// <summary>
        /// Adds a feedback action to the machine
        /// </summary>
        /// <param name="status">VM_Status that triggers feedback</param>
        /// <param name="feedback">action to trigger</param>
        /// <param name="feedbackCase">set to false to trigger feedback when the provided status is not returned</param>
        public void AddFeedback(VM_Status status, Action feedback, bool feedbackCase = true)
        {
            FeedbackActions.Add((status, feedbackCase), feedback);
        }

        /// <summary>
        /// Trigger feedback actions
        /// </summary>
        /// <returns>VM_Status list returned from Validate method</returns>
        public List<VM_Status> Feedback()
        {
            List<VM_Status> output = Validate();
            foreach(var status in FeedbackActions.Keys)
            {
                if (Statuses.Contains(status.Item1))
                {
                    if (status.Item2)
                    {
                        FeedbackActions[status]();
                    }
                }
                else
                {
                    if(!status.Item2)
                    {
                        FeedbackActions[status]();
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Checks all provided validators and returns list of status codes (a return of just VM_Status.OK means validation has passed)
        /// </summary>
        /// <returns>VM_Status list built by each validator function provided</returns>
        public List<VM_Status> Validate()
        {
            Statuses.Clear();
            foreach(var validator in Validators)
            {
                switch (validator.Key)
                {
                    case VM_Type.Alpha:
                        AlphaTest();
                        break;

                    case VM_Type.Num:
                        NumTest();
                        break;

                    case VM_Type.Alpha_Num:
                        AlphaNumTest();
                        break;

                    case VM_Type.Equal:
                        if (validator.Value != null && validator.Value[0] is string equal)
                        {
                            EqualTest(equal);
                        }
                        else
                        {
                            Statuses.Add(VM_Status.Bad_Equal_Args);
                        }
                        break;

                    case VM_Type.Contains:
                        if (validator.Value != null && validator.Value[0] is string contains)
                        {
                            ContainsTest(contains);
                        }
                        else
                        {
                            Statuses.Add(VM_Status.Bad_Contains_Args);
                        }
                        break;

                    case VM_Type.Spec_Char:
                        SpecCharTest();
                        break;

                    case VM_Type.Min_Length:
                        if (validator.Value != null && validator.Value[0] is int min)
                        {
                            MinLenTest(min);
                        }
                        else
                        {
                            Statuses.Add(VM_Status.Bad_Min_Args);
                        }
                        break;

                    case VM_Type.Max_Length:
                        if (validator.Value != null && validator.Value[0] is int max)
                        {
                            MaxLenTest(max);
                        }
                        else
                        {
                            Statuses.Add(VM_Status.Bad_Max_Args);
                        }
                        break;

                    case VM_Type.Omit_Char:
                        if(validator.Value != null && validator.Value[0] is List<char> charList)
                        {
                            InvCharTest(charList);
                        }
                        else if (validator.Value != null && validator.Value[0] is char)
                        {
                            charList = new List<char>();
                            foreach(char c in validator.Value.Select(v => (char)v))
                            {
                                charList.Add(c);
                            }
                            InvCharTest(charList);
                        }
                        else if(validator.Value is null)
                        {
                            InvCharTest();
                        }
                        else
                        {
                            Statuses.Add(VM_Status.Bad_Char_Args);
                        }
                        break;

                    case VM_Type.RegEx:
                        if(validator.Value != null && validator.Value[0] is Regex regex)
                        {
                            RegExTest(regex);
                        }
                        else
                        {
                            Statuses.Add(VM_Status.Bad_RegEx_Args);
                        }
                        break;

                    case VM_Type.Func:
                        if(validator.Value != null && validator.Value[0] is Func<string, bool> func)
                        {
                            FuncTest(func);
                        }
                        else if(validator.Value != null && validator.Value[0] is Func<bool> func1)
                        {
                            FuncTest(func1);
                        }
                        else
                        {
                            Statuses.Add(VM_Status.Bad_Func_Args);
                        }
                        break;
                }               
            }

            if(Statuses.Count == 0)
            {
                Statuses.Add(VM_Status.OK);
            }

            return Statuses;
        }
    }
}
