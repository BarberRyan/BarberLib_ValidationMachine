using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

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


        public void Add(VM_Type validator, params Object[] arguments)
        {
                Validators ??= new Dictionary<VM_Type, Object[]>();
                Validators.Add(validator, arguments);
        }

        private void AlphaTest()
        {
            Regex regex = new("^[a-zA-Z]*$");
            if (!regex.IsMatch(Target.Text))
            {
                Statuses.Add(VM_Status.Not_Alpha);
            }
        }

        private void NumTest()
        {
            Regex regex = new("^[0-9]*$");
            if (!regex.IsMatch(Target.Text))
            {
                Statuses.Add(VM_Status.Not_Num);
            }
        }

        private void AlphaNumTest()
        {
            Regex regex = new("^[a-zA-Z0-9]*$");
            if (!regex.IsMatch(Target.Text))
            {
                Statuses.Add(VM_Status.Not_Alpha_Num);
            }
        }

        private void EqualTest(string test)
        {
            if(Target.Text != test)
            {
                Statuses.Add(VM_Status.Not_Equal);
            }
        }

        private void SpecCharTest()
        {
            Regex regex = new (@"[!@#$%^&*()_+\-=[\]{};':""\\|,.<>/?]");
            if (!regex.IsMatch(Target.Text))
            {
                Statuses.Add(VM_Status.No_Spec_Char);
            }
        }

        private void MinLenTest(int min)
        {
            if(Target.Text.Length < min)
            {
                Statuses.Add(VM_Status.Too_Short);
            }
        }

        private void MaxLenTest(int max)
        {
            if (Target.Text.Length > max)
            {
                Statuses.Add(VM_Status.Too_Long);
            }
        }

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

        private void RegExTest(Regex regex)
        {
            if (!regex.IsMatch(Target.Text))
            {
                Statuses.Add(VM_Status.RegEx_Fail);
            }
        }

        private void FuncTest(Func<bool> func)
        {
            if (!func())
            {
                Statuses.Add(VM_Status.Func_Fail);
            }
        }

        private void FuncTest(Func<string, bool> func)
        {
            if (!func(Target.Text))
            {
                Statuses.Add(VM_Status.Func_Fail);
            }
        }

        public void AddFeedback(VM_Status status, Action feedback, bool feedbackCase = true)
        {
            FeedbackActions.Add((status, feedbackCase), feedback);
        }

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
                        if (validator.Value != null && validator.Value[0] is string test)
                        {
                            EqualTest(test);
                        }
                        else
                        {
                            Statuses.Add(VM_Status.Bad_Equal_Args);
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
