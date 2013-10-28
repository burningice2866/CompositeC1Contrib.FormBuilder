using System.Workflow.Activities;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    partial class AddFieldValidatorWorkflow
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
            System.Workflow.Activities.CodeCondition codecondition1 = new System.Workflow.Activities.CodeCondition();
            this.setStateActivity6 = new System.Workflow.Activities.SetStateActivity();
            this.setStateActivity5 = new System.Workflow.Activities.SetStateActivity();
            this.saveCodeActivity = new System.Workflow.Activities.CodeActivity();
            this.ifElseBranchActivity2 = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifElseBranchActivity1 = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifElseActivity = new System.Workflow.Activities.IfElseActivity();
            this.finishHandleExternalEventActivity = new Composite.C1Console.Workflow.Activities.FinishHandleExternalEventActivity();
            this.setStateActivity4 = new System.Workflow.Activities.SetStateActivity();
            this.cancelHandleExternalEventActivity1 = new Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity();
            this.dataDialogFormActivity = new Composite.C1Console.Workflow.Activities.DataDialogFormActivity();
            this.initCodeActivity = new System.Workflow.Activities.CodeActivity();
            this.setStateActivity2 = new System.Workflow.Activities.SetStateActivity();
            this.DrivenActivity_Ok = new System.Workflow.Activities.EventDrivenActivity();
            this.DrivenActivity_Cancel = new System.Workflow.Activities.EventDrivenActivity();
            this.initializationActivity = new System.Workflow.Activities.StateInitializationActivity();
            this.setStateActivity1 = new System.Workflow.Activities.SetStateActivity();
            this.cancelHandleExternalEventActivity2 = new Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity();
            this.stateInitializationActivity = new System.Workflow.Activities.StateInitializationActivity();
            this.startState = new System.Workflow.Activities.StateActivity();
            this.globalCancelEventDrivenActivity = new System.Workflow.Activities.EventDrivenActivity();
            this.finalState = new System.Workflow.Activities.StateActivity();
            this.initializationState = new System.Workflow.Activities.StateActivity();
            // 
            // setStateActivity6
            // 
            this.setStateActivity6.Name = "setStateActivity6";
            this.setStateActivity6.TargetStateName = "startState";
            // 
            // setStateActivity5
            // 
            this.setStateActivity5.Name = "setStateActivity5";
            this.setStateActivity5.TargetStateName = "finalState";
            // 
            // saveCodeActivity
            // 
            this.saveCodeActivity.Name = "saveCodeActivity";
            this.saveCodeActivity.ExecuteCode += new System.EventHandler(this.saveCodeActivity_ExecuteCode);
            // 
            // ifElseBranchActivity2
            // 
            this.ifElseBranchActivity2.Activities.Add(this.setStateActivity6);
            this.ifElseBranchActivity2.Name = "ifElseBranchActivity2";
            // 
            // ifElseBranchActivity1
            // 
            this.ifElseBranchActivity1.Activities.Add(this.saveCodeActivity);
            this.ifElseBranchActivity1.Activities.Add(this.setStateActivity5);
            codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.validateSave);
            this.ifElseBranchActivity1.Condition = codecondition1;
            this.ifElseBranchActivity1.Name = "ifElseBranchActivity1";
            // 
            // ifElseActivity
            // 
            this.ifElseActivity.Activities.Add(this.ifElseBranchActivity1);
            this.ifElseActivity.Activities.Add(this.ifElseBranchActivity2);
            this.ifElseActivity.Name = "ifElseActivity";
            // 
            // finishHandleExternalEventActivity
            // 
            this.finishHandleExternalEventActivity.EventName = "Finish";
            this.finishHandleExternalEventActivity.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
            this.finishHandleExternalEventActivity.Name = "finishHandleExternalEventActivity";
            // 
            // setStateActivity4
            // 
            this.setStateActivity4.Name = "setStateActivity4";
            this.setStateActivity4.TargetStateName = "finalState";
            // 
            // cancelHandleExternalEventActivity1
            // 
            this.cancelHandleExternalEventActivity1.EventName = "Cancel";
            this.cancelHandleExternalEventActivity1.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
            this.cancelHandleExternalEventActivity1.Name = "cancelHandleExternalEventActivity1";
            // 
            // dataDialogFormActivity
            // 
            this.dataDialogFormActivity.ContainerLabel = "";
            this.dataDialogFormActivity.FormDefinitionFileName = "\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddFieldValidatorWorkfl" +
                "ow.xml";
            this.dataDialogFormActivity.Name = "dataDialogFormActivity";
            // 
            // initCodeActivity
            // 
            this.initCodeActivity.Name = "initCodeActivity";
            this.initCodeActivity.ExecuteCode += new System.EventHandler(this.initCodeActivity_ExecuteCode);
            // 
            // setStateActivity2
            // 
            this.setStateActivity2.Name = "setStateActivity2";
            this.setStateActivity2.TargetStateName = "startState";
            // 
            // DrivenActivity_Ok
            // 
            this.DrivenActivity_Ok.Activities.Add(this.finishHandleExternalEventActivity);
            this.DrivenActivity_Ok.Activities.Add(this.ifElseActivity);
            this.DrivenActivity_Ok.Name = "DrivenActivity_Ok";
            // 
            // DrivenActivity_Cancel
            // 
            this.DrivenActivity_Cancel.Activities.Add(this.cancelHandleExternalEventActivity1);
            this.DrivenActivity_Cancel.Activities.Add(this.setStateActivity4);
            this.DrivenActivity_Cancel.Name = "DrivenActivity_Cancel";
            // 
            // initializationActivity
            // 
            this.initializationActivity.Activities.Add(this.initCodeActivity);
            this.initializationActivity.Activities.Add(this.dataDialogFormActivity);
            this.initializationActivity.Name = "initializationActivity";
            // 
            // setStateActivity1
            // 
            this.setStateActivity1.Name = "setStateActivity1";
            this.setStateActivity1.TargetStateName = "finalState";
            // 
            // cancelHandleExternalEventActivity2
            // 
            this.cancelHandleExternalEventActivity2.EventName = "Cancel";
            this.cancelHandleExternalEventActivity2.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
            this.cancelHandleExternalEventActivity2.Name = "cancelHandleExternalEventActivity2";
            // 
            // stateInitializationActivity
            // 
            this.stateInitializationActivity.Activities.Add(this.setStateActivity2);
            this.stateInitializationActivity.Name = "stateInitializationActivity";
            // 
            // startState
            // 
            this.startState.Activities.Add(this.initializationActivity);
            this.startState.Activities.Add(this.DrivenActivity_Cancel);
            this.startState.Activities.Add(this.DrivenActivity_Ok);
            this.startState.Name = "startState";
            // 
            // globalCancelEventDrivenActivity
            // 
            this.globalCancelEventDrivenActivity.Activities.Add(this.cancelHandleExternalEventActivity2);
            this.globalCancelEventDrivenActivity.Activities.Add(this.setStateActivity1);
            this.globalCancelEventDrivenActivity.Name = "globalCancelEventDrivenActivity";
            // 
            // finalState
            // 
            this.finalState.Name = "finalState";
            // 
            // initializationState
            // 
            this.initializationState.Activities.Add(this.stateInitializationActivity);
            this.initializationState.Name = "initializationState";
            // 
            // AddFieldValidatorWorkflow
            // 
            this.Activities.Add(this.initializationState);
            this.Activities.Add(this.finalState);
            this.Activities.Add(this.globalCancelEventDrivenActivity);
            this.Activities.Add(this.startState);
            this.CompletedStateName = "finalState";
            this.DynamicUpdateCondition = null;
            this.InitialStateName = "initializationState";
            this.Name = "AddFieldValidatorWorkflow";
            this.CanModifyActivities = false;

        }

        #endregion

        private EventDrivenActivity globalCancelEventDrivenActivity;

        private StateActivity finalState;

        private Composite.C1Console.Workflow.Activities.DataDialogFormActivity dataDialogFormActivity;

        private CodeActivity initCodeActivity;

        private SetStateActivity setStateActivity2;

        private SetStateActivity setStateActivity1;

        private StateInitializationActivity stateInitializationActivity;

        private SetStateActivity setStateActivity4;

        private Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity cancelHandleExternalEventActivity1;

        private Composite.C1Console.Workflow.Activities.FinishHandleExternalEventActivity finishHandleExternalEventActivity;

        private EventDrivenActivity DrivenActivity_Cancel;

        private EventDrivenActivity DrivenActivity_Ok;

        private Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity cancelHandleExternalEventActivity2;

        private SetStateActivity setStateActivity6;

        private SetStateActivity setStateActivity5;

        private IfElseBranchActivity ifElseBranchActivity2;

        private IfElseBranchActivity ifElseBranchActivity1;

        private IfElseActivity ifElseActivity;

        private CodeActivity saveCodeActivity;

        private StateInitializationActivity initializationActivity;

        private StateActivity startState;

        private StateActivity initializationState;

























































































    }
}
