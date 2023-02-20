# ValidationMachine by Ryan Barber

To add validation to a control, create a machine factory in your form class and then call AddValidator.

	VM_Factory factory = new VM_Factory();
	factory.AddValidator([text-based control], VM_Type.[Type of validator], [Optional arguments]);


To add feedback, we use AddFeedback.

	factory.AddFeedback([control being validated], VM_Status.[status to react to], [Method name or Lambda expression to trigger for feedback], [optional boolean, set to false to trigger feedback when the provided status is not returned])
