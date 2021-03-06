using GitVersion;
using LibGit2Sharp;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
public class ReleaseTests : Lg2sHelperBase
{
    [Test]
    public void No_commits_with_tag()
    {
        var repoPath = Clone(ASBMTestRepoWorkingDirPath);
        using (var repo = new Repository(repoPath))
        {
            const string branchName = "release-0.3.0";

            var branchingCommit = repo.Branches["develop"].Tip;
            var releaseBranch = repo.Branches.Add(branchName, branchingCommit);

            var sign = Constants.SignatureNow();
            repo.Tags.Add("0.3.0-alpha5", branchingCommit, sign, "release");

            var finder = new ReleaseVersionFinder();

            var version = finder.FindVersion(new GitVersionContext
            {
                Repository = repo,
                CurrentBranch = releaseBranch,
            });
            Assert.IsNull(version.Version.PreReleasePartTwo, "PreReleasePartTwo null since there is no commits");

            ObjectApprover.VerifyWithJson(version, Scrubbers.GuidScrubber);
        }
    }

    [Test]
    public void No_commits_with_no_tag()
    {
        var repoPath = Clone(ASBMTestRepoWorkingDirPath);
        using (var repo = new Repository(repoPath))
        {
            const string branchName = "release-0.3.0";

            var branchingCommit = repo.Branches["develop"].Tip;
            var releaseBranch = repo.Branches.Add(branchName, branchingCommit);

            var finder = new ReleaseVersionFinder();

            var version = finder.FindVersion(new GitVersionContext
            {
                CurrentBranch = releaseBranch,
                Repository = repo
            });

            Assert.IsNull(version.Version.PreReleasePartTwo, "PreReleasePartTwo null since there is no commits");
            ObjectApprover.VerifyWithJson(version, Scrubbers.GuidScrubber);
        }
    }

    [Test]
    public void First_commit_no_tag()
    {
        var repoPath = Clone(ASBMTestRepoWorkingDirPath);
        using (var repo = new Repository(repoPath))
        {
            const string branchName = "release-0.5.0";

            var branchingCommit = repo.Branches["develop"].Tip;
            repo.Branches.Add(branchName, branchingCommit).Checkout();

            AddOneCommitToHead(repo, "first commit on release");

            var releaseBranch = repo.Branches[branchName];

            var finder = new ReleaseVersionFinder();

            var version = finder.FindVersion(new GitVersionContext
            {
                Repository = repo,
                CurrentBranch = releaseBranch,
            });
            Assert.AreEqual(1, version.Version.PreReleasePartTwo, "PreReleasePartTwo should be set to 1 since there is 1 commit");
            ObjectApprover.VerifyWithJson(version, Scrubbers.GuidScrubber);
        }
    }

    [Test]
    public void First_commit_with_tag_on_branching_commit()
    {
        var repoPath = Clone(ASBMTestRepoWorkingDirPath);
        using (var repo = new Repository(repoPath))
        {
            const string branchName = "release-0.5.0";

            var branchingCommit = repo.Branches["develop"].Tip;
            repo.Branches.Add(branchName, branchingCommit).Checkout();

            AddOneCommitToHead(repo, "first commit on release");

            var releaseBranch = repo.Branches[branchName];

            var sign = Constants.SignatureNow();
            repo.Tags.Add("0.5.0-alpha5", branchingCommit, sign, "release");

            var finder = new ReleaseVersionFinder();

            var version = finder.FindVersion(new GitVersionContext
            {
                Repository = repo,
                CurrentBranch = releaseBranch,
            });
            Assert.AreEqual(1, version.Version.PreReleasePartTwo, "PreReleasePartTwo should be set to 1 since there is 1 commit");

            ObjectApprover.VerifyWithJson(version, Scrubbers.GuidScrubber);
        }
    }


    [Test]
    public void First_commit_with_tag_on_new_commit()
    {
        var repoPath = Clone(ASBMTestRepoWorkingDirPath);
        using (var repo = new Repository(repoPath))
        {
            const string branchName = "release-0.5.0";

            var branchingCommit = repo.Branches["develop"].Tip;
            repo.Branches.Add(branchName, branchingCommit).Checkout();

            var firstCommit = AddOneCommitToHead(repo, "first commit on release");

            var releaseBranch = repo.Branches[branchName];

            var sign = Constants.SignatureNow();
            repo.Tags.Add("0.5.0-alpha5", firstCommit, sign, "release");

            var finder = new ReleaseVersionFinder();

            var version = finder.FindVersion(new GitVersionContext
            {
                Repository = repo,
                CurrentBranch = releaseBranch,
            });
            Assert.IsNull(version.Version.PreReleasePartTwo, "PreReleasePartTwo null since the tag takes precedence");
            ObjectApprover.VerifyWithJson(version, Scrubbers.GuidScrubber);
        }
    }

    [Test]
    public void Second_commit()
    {
        var repoPath = Clone(ASBMTestRepoWorkingDirPath);
        using (var repo = new Repository(repoPath))
        {
            const string branchName = "release-0.4.0";

            var branchingCommit = repo.Branches["develop"].Tip;
            repo.Branches.Add(branchName, branchingCommit).Checkout();

            AddOneCommitToHead(repo, "first commit on release");
            AddOneCommitToHead(repo, "second commit on release");

            var releaseBranch = repo.Branches[branchName];

            var sign = Constants.SignatureNow();
            repo.Tags.Add("0.4.0-alpha5", branchingCommit, sign, "release");

            var finder = new ReleaseVersionFinder();

            var version = finder.FindVersion(new GitVersionContext
            {
                Repository = repo,
                CurrentBranch = releaseBranch,
            });
            Assert.AreEqual(2, version.Version.PreReleasePartTwo, "PreReleasePartTwo should be set to 2 since there is 2 commits on the branch");
            ObjectApprover.VerifyWithJson(version, Scrubbers.GuidScrubber);
        }
    }

    [Test]
    public void Second_commit_with_no_tag()
    {
        var repoPath = Clone(ASBMTestRepoWorkingDirPath);
        using (var repo = new Repository(repoPath))
        {
            const string branchName = "release-0.4.0";

            var branchingCommit = repo.Branches["develop"].Tip;
            repo.Branches.Add(branchName, branchingCommit).Checkout();

            AddOneCommitToHead(repo, "first commit on release");
            AddOneCommitToHead(repo, "second commit on release");

            var releaseBranch = repo.Branches[branchName];

            var finder = new ReleaseVersionFinder();

            var version = finder.FindVersion(new GitVersionContext
            {
                Repository = repo,
                CurrentBranch = releaseBranch,
            });
            Assert.AreEqual(2, version.Version.PreReleasePartTwo, "PreReleasePartTwo should be set to 2 since there is 2 commits on the branch");
            ObjectApprover.VerifyWithJson(version, Scrubbers.GuidScrubber);
        }
    }

    [Test]
    public void Override_stage_using_tag()
    {
        var repoPath = Clone(ASBMTestRepoWorkingDirPath);
        using (var repo = new Repository(repoPath))
        {
            const string branchName = "release-0.4.0";

            var branchingCommit = repo.Branches["develop"].Tip;
            repo.Branches.Add(branchName, branchingCommit).Checkout();

            AddOneCommitToHead(repo, "first commit on release");

            var releaseBranch = repo.Branches[branchName];

            var sign = Constants.SignatureNow();
            repo.Tags.Add("0.4.0-RC4", branchingCommit, sign, "release");

            var finder = new ReleaseVersionFinder();

            var version = finder.FindVersion(new GitVersionContext
            {
                Repository = repo,
                CurrentBranch = releaseBranch,
            });
            //tag: 0.4.0-RC1 => 
            ObjectApprover.VerifyWithJson(version, Scrubbers.GuidScrubber);
        }
    }

    [Test]
    public void EnsureAReleaseBranchNameDoesNotExposeAPatchSegment()
    {
        var repoPath = Clone(ASBMTestRepoWorkingDirPath);
        using (var repo = new Repository(repoPath))
        {
            const string branchName = "release-0.3.1";

            var branchingCommit = repo.Branches["develop"].Tip;
            var releaseBranch = repo.Branches.Add(branchName, branchingCommit);

            var finder = new ReleaseVersionFinder();

            Assert.Throws<ErrorException>(() => finder.FindVersion(new GitVersionContext
            {
                Repository = repo,
                CurrentBranch = releaseBranch,
            }));
        }
    }

    [Test]
    public void EnsureAReleaseBranchNameDoesNotExposeAStability()
    {
        var repoPath = Clone(ASBMTestRepoWorkingDirPath);
        using (var repo = new Repository(repoPath))
        {
            const string branchName = "release-0.3.0-Final";

            var branchingCommit = repo.Branches["develop"].Tip;
            var releaseBranch = repo.Branches.Add(branchName, branchingCommit);

            var finder = new ReleaseVersionFinder();

            Assert.Throws<ErrorException>(() => finder.FindVersion(new GitVersionContext
            {
                Repository = repo,
                CurrentBranch = releaseBranch,
            }));
        }
    }
    //TODO:
    //[Test]
    //[ExpectedException]
    //public void Override_stage_using_tag_should_throw_on_version_mismatch()
    //{
    //    var version = FinderWrapper.FindVersionForCommit("34dbc768fcbdd57d6089fe28f9d37472b9e97e35", "release-0.5.0");
    //}

    [Test, Ignore("Not really going to happen in real life se we skip this for now")]
    public void After_merge_to_master()
    {
        //TODO
        //Assert.Throws<Exception>(() => FinderWrapper.FindVersionForCommit("TODO", "release-0.5.0"));
    }

}