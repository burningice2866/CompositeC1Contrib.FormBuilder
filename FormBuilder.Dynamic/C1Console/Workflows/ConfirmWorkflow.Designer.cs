using System.Workflow.Activities;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    partial class ConfirmWorkflow
    {
        #region Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        private void InitializeComponent()
        {
            this.CanModifyActivities = true;
            this.setStateActivity2 = new System.Workflow.Activities.SetStateActivity();
            this.codeActivity = new System.Workflow.Activities.CodeActivity();
            this.setStateActivity1 = new System.Workflow.Activities.SetStateActivity();
            this.cancelHandleExternalEventActivity1 = new Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity();
            this.setStateActivity3 = new System.Workflow.Activities.SetStateActivity();
            this.finishHandleExternalEventActivity1 = new Composite.C1Console.Workflow.Activities.FinishHandleExternalEventActivity();
            this.confirmDialogFormActivity = new Composite.C1Console.Workflow.Activities.ConfirmDialogFormActivity();
            this.intiCodeActivity = new System.Workflow.Activities.CodeActivity();
            this.executeStateInitializationActivity = new System.Workflow.Activities.StateInitializationActivity();
            this.setStateActivity4 = new System.Workflow.Activities.SetStateActivity();
            this.cancelHandleExternalEventActivity2 = new Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity();
            this.EventDrivenActivity_Cancel = new System.Workflow.Activities.EventDrivenActivity();
            this.EventDrivenActivity_Finish = new System.Workflow.Activities.EventDrivenActivity();
            this.stateInitializationActivity = new System.Workflow.Activities.StateInitializationActivity();
            this.executeState = new System.Workflow.Activities.StateActivity();
            this.GlobalEventDrivenActivity = new System.Workflow.Activities.EventDrivenActivity();
            this.finalState = new System.Workflow.Activities.StateActivity();
            this.initializationState = new System.Workflow.Activities.StateActivity();
            // 
            // setStateActivity2
            // 
            this.setStateActivity2.Name = "setStateActivity2";
            this.setStateActivity2.TargetStateName = "finalState";
            // 
            // codeActivity
            // 
            this.codeActivity.Name = "codeActivity";
            this.codeActivity.ExecuteCode += new System.EventHandler(this.codeActivity_ExecuteCode);
            // 
            // setStateActivity1
            // 
            this.setStateActivity1.Name = "setStateActivity1";
            this.setStateActivity1.TargetStateName = "finalState";
            // 
            // cancelHandleExternalEventActivity1
            // 
            this.cancelHandleExternalEventActivity1.EventName = "Cancel";
            this.cancelHandleExternalEventActivity1.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
            this.cancelHandleExternalEventActivity1.Name = "cancelHandleExternalEventActivity1";
            // 
            // setStateActivity3
            // 
            this.setStateActivity3.Name = "setStateActivity3";
            this.setStateActivity3.TargetStateName = "executeState";
            // 
            // finishHandleExternalEventActivity1
            // 
            this.finishHandleExternalEventActivity1.EventName = "Finish";
            this.finishHandleExternalEventActivity1.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
            this.finishHandleExternalEventActivity1.Name = "finishHandleExternalEventActivity1";
            // 
            // confirmDialogFormActivity
            // 
            this.confirmDialogFormActivity.ContainerLabel = null;
            this.confirmDialogFormActivity.FormDefinitionFileName = "\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\ConfirmMessage.xml";
            this.confirmDialogFormActivity.Name = "confirmDialogFormActivity";
            // 
            // intiCodeActivity
            // 
            this.intiCodeActivity.Name = "intiCodeActivity";
            this.intiCodeActivity.ExecuteCode += new System.EventHandler(this.initCodeActivity_ExecuteCode);
            // 
            // executeStateInitializationActivity
            // 
            this.executeStateInitializationActivity.Activities.Add(this.codeActivity);
            this.executeStateInitializationActivity.Activities.Add(this.setStateActivity2);
            this.executeStateInitializationActivity.Name = "executeStateInitializationActivity";
            // 
            // setStateActivity4
            // 
            this.setStateActivity4.Name = "setStateActivity4";
            this.setStateActivity4.TargetStateName = "finalState";
            // 
            // cancelHandleExternalEventActivity2
            // 
            this.cancelHandleExternalEventActivity2.EventName = "Cancel";
            this.cancelHandleExternalEventActivity2.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
            this.cancelHandleExternalEventActivity2.Name = "cancelHandleExternalEventActivity2";
            // 
            // EventDrivenActivity_Cancel
            // 
            this.EventDrivenActivity_Cancel.Activities.Add(this.cancelHandleExternalEventActivity1);
            this.EventDrivenActivity_Cancel.Activities.Add(this.setStateActivity1);
            this.EventDrivenActivity_Cancel.Name = "EventDrivenActivity_Cancel";
            // 
            // EventDrivenActivity_Finish
            // 
            this.EventDrivenActivity_Finish.Activities.Add(this.finishHandleExternalEventActivity1);
            this.EventDrivenActivity_Finish.Activities.Add(this.setStateActivity3);
            this.EventDrivenActivity_Finish.Name = "EventDrivenActivity_Finish";
            // 
            // stateInitializationActivity
            // 
            this.stateInitializationActivity.Activities.Add(this.intiCodeActivity);
            this.stateInitializationActivity.Activities.Add(this.confirmDialogFormActivity);
            this.stateInitializationActivity.Name = "stateInitializationActivity";
            // 
            // executeState
            // 
            this.executeState.Activities.Add(this.executeStateInitializationActivity);
            this.executeState.Name = "executeState";
            // 
            // GlobalEventDrivenActivity
            // 
            this.GlobalEventDrivenActivity.Activities.Add(this.cancelHandleExternalEventActivity2);
            this.GlobalEventDrivenActivity.Activities.Add(this.setStateActivity4);
            this.GlobalEventDrivenActivity.Name = "GlobalEventDrivenActivity";
            // 
            // finalState
            // 
            this.finalState.Name = "finalState";
            // 
            // initializationState
            // 
            this.initializationState.Activities.Add(this.stateInitializationActivity);
            this.initializationState.Activities.Add(this.EventDrivenActivity_Finish);
            this.initializationState.Activities.Add(this.EventDrivenActivity_Cancel);
            this.initializationState.Name = "initializationState";
            // 
            // ConfirmWorkflow
            // 
            this.Activities.Add(this.initializationState);
            this.Activities.Add(this.finalState);
            this.Activities.Add(this.GlobalEventDrivenActivity);
            this.Activities.Add(this.executeState);
            this.CompletedStateName = "finalState";
            this.DynamicUpdateCondition = null;
            this.InitialStateName = "initializationState";
            this.Name = "ConfirmWorkflow";
            this.CanModifyActivities = false;

        }

        #endregion

        private Composite.C1Console.Workflow.Activities.FinishHandleExternalEventActivity finishHandleExternalEventActivity1;

        private SetStateActivity setStateActivity4;

        private Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity cancelHandleExternalEventActivity2;

        private EventDrivenActivity EventDrivenActivity_Finish;

        private StateInitializationActivity stateInitializationActivity;

        private EventDrivenActivity GlobalEventDrivenActivity;

        private StateActivity finalState;

        private Composite.C1Console.Workflow.Activities.ConfirmDialogFormActivity confirmDialogFormActivity;

        private EventDrivenActivity EventDrivenActivity_Cancel;

        private CodeActivity codeActivity;

        private SetStateActivity setStateActivity1;

        private Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity cancelHandleExternalEventActivity1;

        private StateInitializationActivity executeStateInitializationActivity;

        private StateActivity executeState;

        private SetStateActivity setStateActivity2;

        private SetStateActivity setStateActivity3;

        private CodeActivity intiCodeActivity;

        private StateActivity initializationState;





























    }
}
