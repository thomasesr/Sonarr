using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.MediaFiles.EpisodeImport;
using NzbDrone.Core.Test.Framework;

namespace NzbDrone.Core.Test.MediaFiles.EpisodeImport
{
    [TestFixture]
    public class RejectedFileExtensionServiceFixture : CoreTest<RejectedFileExtensionService>
    {
        private void GivenUserRejectedExtensions(string extensions)
        {
            Mocker.GetMock<IConfigService>()
                  .SetupGet(s => s.UserRejectedExtensions)
                  .Returns(extensions);
        }

        private void GivenVideoFileExtensions(string extensions)
        {
            Mocker.GetMock<IConfigService>()
                  .SetupGet(s => s.VideoFileExtensions)
                  .Returns(extensions);
        }

        [Test]
        public void should_not_return_rejection_for_media_files()
        {
            Subject.GetRejection(new[] { @"C:\Test\Droned.S01E01.mkv", @"C:\Test\Droned.S01E01.srt" }).Should().BeNull();
        }

        [Test]
        public void should_not_return_rejection_for_files_without_an_extension()
        {
            Subject.GetRejection(new[] { @"C:\Test\Droned.S01E01" }).Should().BeNull();
        }

        [Test]
        public void should_return_rejection_for_dangerous_file()
        {
            var rejection = Subject.GetRejection(new[] { @"C:\Test\Droned.S01E01.mkv", @"C:\Test\Droned.S01E01.lnk" });

            rejection.Should().NotBeNull();
            rejection.Reason.Should().Be(ImportRejectionReason.DangerousFile);
            rejection.Message.Should().Contain(".lnk");
        }

        [Test]
        public void should_return_rejection_for_executable_file()
        {
            var rejection = Subject.GetRejection(new[] { "Droned.S01E01/Droned.S01E01.bat" });

            rejection.Should().NotBeNull();
            rejection.Reason.Should().Be(ImportRejectionReason.ExecutableFile);
            rejection.Message.Should().Contain(".bat");
        }

        [TestCase("iso")]
        [TestCase(".iso")]
        [TestCase("mp3, iso")]
        public void should_return_rejection_for_user_defined_extension(string userRejectedExtensions)
        {
            GivenUserRejectedExtensions(userRejectedExtensions);

            var rejection = Subject.GetRejection(new[] { @"C:\Test\Droned.S01E01.ISO" });

            rejection.Should().NotBeNull();
            rejection.Reason.Should().Be(ImportRejectionReason.UserRejectedExtension);
            rejection.Message.Should().Contain(".ISO");
        }

        [Test]
        public void should_not_return_rejection_when_user_defined_extensions_are_not_configured()
        {
            GivenUserRejectedExtensions(null);

            Subject.GetRejection(new[] { @"C:\Test\Droned.S01E01.iso" }).Should().BeNull();
        }

        [Test]
        public void should_not_return_video_rejection_when_media_file_present()
        {
            Subject.GetVideoFileRejection(new[] { @"C:\Test\Droned.S01E01.mkv", @"C:\Test\Droned.S01E01.srt", @"C:\Test\Droned.S01E01.nfo" }).Should().BeNull();
        }

        [Test]
        public void should_return_video_rejection_when_no_media_file_present()
        {
            var rejection = Subject.GetVideoFileRejection(new[] { @"C:\Test\Droned.S01E01.srt", @"C:\Test\Droned.S01E01.nfo" });

            rejection.Should().NotBeNull();
            rejection.Reason.Should().Be(ImportRejectionReason.MissingVideoFile);
        }

        [Test]
        public void should_not_return_video_rejection_when_media_file_present_using_user_defined_extensions()
        {
            GivenVideoFileExtensions("mp4");

            Subject.GetVideoFileRejection(new[] { @"C:\Test\Droned.S01E01.mp4" }).Should().BeNull();
        }

        [Test]
        public void should_return_video_rejection_when_no_media_file_present_using_user_defined_extensions()
        {
            GivenVideoFileExtensions("mp4");

            var rejection = Subject.GetVideoFileRejection(new[] { @"C:\Test\Droned.S01E01.mkv" });

            rejection.Should().NotBeNull();
            rejection.Reason.Should().Be(ImportRejectionReason.MissingVideoFile);
        }
    }
}
