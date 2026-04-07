using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;
using Stateless;
using System;

namespace BugTests;

[TestClass]
public class BugTests
{
    [TestMethod]
    public void Constructor_DefaultState_ShouldBeNew()
    {
        var bug = new Bug();

        Assert.AreEqual(Bug.State.New, bug.CurrentState);
    }

    [TestMethod]
    public void Constructor_CustomInitialState_ShouldBeSpecifiedState()
    {
        var bug = new Bug(Bug.State.Reopened);

        Assert.AreEqual(Bug.State.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void Assign_FromNew_ShouldChangeToAssigned()
    {
        var bug = new Bug();

        bug.Assign("Developer");

        Assert.AreEqual(Bug.State.Assigned, bug.CurrentState);
    }

    [TestMethod]
    public void StartWork_FromAssigned_ShouldChangeToInProgress()
    {
        var bug = new Bug();
        bug.Assign("Developer");

        bug.StartWork();

        Assert.AreEqual(Bug.State.InProgress, bug.CurrentState);
    }

    [TestMethod]
    public void Fix_FromInProgress_ShouldChangeToFixed()
    {
        var bug = new Bug();
        bug.Assign("Developer");
        bug.StartWork();

        bug.Fix();

        Assert.AreEqual(Bug.State.Fixed, bug.CurrentState);
    }

    [TestMethod]
    public void Test_FromFixed_ShouldChangeToReadyForTest()
    {
        var bug = new Bug();
        bug.Assign("Developer");
        bug.StartWork();
        bug.Fix();

        bug.Test();

        Assert.AreEqual(Bug.State.ReadyForTest, bug.CurrentState);
    }

    [TestMethod]
    public void Close_FromReadyForTest_ShouldChangeToClosed()
    {
        var bug = new Bug();
        bug.Assign("Developer");
        bug.StartWork();
        bug.Fix();
        bug.Test();

        bug.Close();

        Assert.AreEqual(Bug.State.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void Reopen_FromClosed_ShouldChangeToReopened()
    {
        var bug = new Bug();
        bug.Assign("Developer");
        bug.StartWork();
        bug.Fix();
        bug.Test();
        bug.Close();

        bug.Reopen();

        Assert.AreEqual(Bug.State.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void Reject_FromNew_ShouldChangeToRejected()
    {
        var bug = new Bug();

        bug.Reject();

        Assert.AreEqual(Bug.State.Rejected, bug.CurrentState);
    }

    [TestMethod]
    public void NeedMoreInfo_FromNew_ShouldChangeToNeedInfo()
    {
        var bug = new Bug();

        bug.NeedMoreInfo();

        Assert.AreEqual(Bug.State.NeedInfo, bug.CurrentState);
    }

    [TestMethod]
    public void Assign_FromRejected_ShouldChangeToAssigned()
    {
        var bug = new Bug();
        bug.Reject();

        bug.Assign("Developer");

        Assert.AreEqual(Bug.State.Assigned, bug.CurrentState);
    }

    [TestMethod]
    public void Assign_FromNeedInfo_ShouldChangeToAssigned()
    {
        var bug = new Bug();
        bug.NeedMoreInfo();

        bug.Assign("Developer");

        Assert.AreEqual(Bug.State.Assigned, bug.CurrentState);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Close_FromNew_ShouldThrowException()
    {
        var bug = new Bug();

        bug.Close();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Fix_FromNew_ShouldThrowException()
    {
        var bug = new Bug();

        bug.Fix();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_FromNew_ShouldThrowException()
    {
        var bug = new Bug();

        bug.Test();
    }

    [TestMethod]
    public void CanAssign_FromNew_ShouldReturnTrue()
    {
        var bug = new Bug();

        Assert.IsTrue(bug.CanAssign);
    }

    [TestMethod]
    public void CanAssign_FromInProgress_ShouldReturnFalse()
    {
        var bug = new Bug();
        bug.Assign("Developer");
        bug.StartWork();

        Assert.IsFalse(bug.CanAssign);
    }

    [TestMethod]
    public void CanClose_FromReadyForTest_ShouldReturnTrue()
    {
        var bug = new Bug();
        bug.Assign("Developer");
        bug.StartWork();
        bug.Fix();
        bug.Test();

        Assert.IsTrue(bug.CanClose);
    }

    [TestMethod]
    public void CanClose_FromNew_ShouldReturnFalse()
    {
        var bug = new Bug();

        Assert.IsFalse(bug.CanClose);
    }

    [TestMethod]
    public void CanReopen_FromClosed_ShouldReturnTrue()
    {

        var bug = new Bug();
        bug.Assign("Developer");
        bug.StartWork();
        bug.Fix();
        bug.Test();
        bug.Close();

        Assert.IsTrue(bug.CanReopen);
    }

    [TestMethod]
    public void FullWorkflow_ShouldCompleteSuccessfully()
    {
        var bug = new Bug();

        bug.Assign("Developer");
        Assert.AreEqual(Bug.State.Assigned, bug.CurrentState);

        bug.StartWork();
        Assert.AreEqual(Bug.State.InProgress, bug.CurrentState);

        bug.Fix();
        Assert.AreEqual(Bug.State.Fixed, bug.CurrentState);

        bug.Test();
        Assert.AreEqual(Bug.State.ReadyForTest, bug.CurrentState);

        bug.Close();
        Assert.AreEqual(Bug.State.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void ReopenAndAssign_ShouldWorkCorrectly()
    {
        var bug = new Bug();
        bug.Assign("Dev1");
        bug.StartWork();
        bug.Fix();
        bug.Test();
        bug.Close();

        bug.Reopen();
        bug.Assign("Dev2");
        bug.StartWork();

        Assert.AreEqual(Bug.State.InProgress, bug.CurrentState);
    }

    [TestMethod]
    public void MultipleReopens_ShouldBePossible()
    {
        var bug = new Bug();
        bug.Assign("Developer");
        bug.StartWork();
        bug.Fix();
        bug.Test();
        bug.Close();

        bug.Reopen();
        Assert.AreEqual(Bug.State.Reopened, bug.CurrentState);

        bug.Assign("Developer");
        bug.StartWork();
        bug.Fix();
        bug.Test();
        bug.Close();

        Assert.AreEqual(Bug.State.Closed, bug.CurrentState);
    }
}