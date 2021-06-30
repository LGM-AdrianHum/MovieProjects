using System.IO;
using System.Linq;
using PropertyChanged;

namespace TvFileBot.ViewModel
{
    [ImplementPropertyChanged]
    public class MoveFileDetails
    {
        private ItemBase _o;

        public MoveFileDetails()
        { }
        public MoveFileDetails(ItemBase o)
        {
            _o = o;
            SourceTargetFile = o.FullName;
            MoveTargetDirectory = o.TargetDirectoryFullname;
            MoveTargetDirectorySeasonCode = "S" + o.Season.ToString("#00");
            var ext = Path.GetExtension(o.Name) ?? "";
            MoveTargetFilename =
                $"{o.MatchingShow.SeriesInformation.Series.Name.ToCleanString()} - {o.SeasonEpisode} - {o.EpisodeName.ToCleanString()}";
            var wd = Path.Combine(MoveTargetDirectory, MoveTargetDirectorySeasonCode);
            var gi = new FileInfo(o.FullName);
            MoveTargetDetails = PossibleConflictDetails = $"{gi.CreationTime.ToString("dd-MMM-yyyy HH:mm")}  {gi.Length.ToBytes()}";
            if (Directory.Exists(wd))
            {
                var di = new DirectoryInfo(wd);
                var fis = di.GetFiles(MoveTargetFilename + ".*");
                if (fis.Any())
                {
                    var fi = fis.FirstOrDefault();
                    if (fi != null)
                    {
                        PossibleConflictName = fi.Name;
                        PossibleConflictDetails = $"{fi.CreationTime.ToString("dd-MMM-yyyy HH:mm")}  {fi.Length.ToBytes()}";
                    }
                }
            }
            MoveTargetFilename += ext;
        }

        public string SourceTargetFile { get; set; }

        public void Rename()
        {
            var loc = Path.GetDirectoryName(SourceTargetFile);
            var newname = Path.Combine(loc, MoveTargetFilename);
            File.Move(SourceTargetFile,newname);
            _o.FullName = newname;
            _o.Name = MoveTargetFilename;
        }

        public void MoveLocalLibrary()
        {
            var root1 = Path.GetPathRoot(MoveTargetDirectory);
            var root2 = Path.GetPathRoot(SourceTargetFile);
            if (string.IsNullOrEmpty(root1) || string.IsNullOrEmpty(root2)) return;
            var localtargetdirectory = MoveTargetDirectory.Replace(root1, root2);
            if (!Directory.Exists(localtargetdirectory)) Directory.CreateDirectory(localtargetdirectory);
            var newname = Path.Combine(localtargetdirectory, MoveTargetFilename);
            File.Move(SourceTargetFile, newname);
        }

        public string MoveTargetDirectory { get; set; }
        public string MoveTargetDirectorySeasonCode { get; set; }
        public string MoveTargetFilename { get; set; }
        public string MoveTargetDetails { get; set; }
        public string PossibleConflictName { get; set; }
        public string PossibleConflictDetails { get; set; }
    }
}