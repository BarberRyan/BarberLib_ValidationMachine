using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ValidationMachine
{    
    public class VM_Factory
    {
        public Dictionary<Control, VMachine> VMachines = new Dictionary<Control, VMachine>();

        private void AddMachine(Control target)
        {
            VMachine newMachine = new VMachine(target);

            VMachines.Add(target, newMachine);
        }

        public void AddValidator(Control target, VM_Type validator, params Object[] args)
        {
            if(VMachines == null)
            {
                VMachines = new Dictionary<Control, VMachine>();
            }
            if (VMachines.ContainsKey(target))
            {
                VMachines[target].Add(validator, args);
            }
            else
            {
                AddMachine(target);
                AddValidator(target, validator, args);
            }
        }

        public void AddFeedback(Control target, VM_Status status, Action feedback, bool feedbackCase = true)
        {
            if (VMachines.ContainsKey(target))
            {
                VMachines[target].AddFeedback(status, feedback, feedbackCase);
            }
        }

        public List<(Control, List<VM_Status>)> ValidateAll(bool feedback = false)
        {
            List<(Control, List<VM_Status>)> output = new List<(Control, List<VM_Status>)>();

            foreach(Control control in VMachines.Keys)
            {
                List<VM_Status> statuses = new List<VM_Status>();
                if (feedback)
                {
                    statuses = VMachines[control].Feedback();
                }
                else
                {
                    statuses = VMachines[control].Validate();
                }
                output.Add((control, statuses));
            }
            return output;
        }
    }
}
