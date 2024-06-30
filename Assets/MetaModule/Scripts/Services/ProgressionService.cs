using Core.Services;
using Infrastructure.Settings;
using PairFindingGame;

namespace Infrastructure.Services
{
    public class ProgressionService : IInitializableService
    {
        private ConfigurationService _configurationService;
        private LevelsService _levelsService;
        private ProgressionSettings _progressionSettings;
        
        public void Initialize()
        {
            _configurationService = ServiceLocator.GetService<ConfigurationService>();
            _levelsService = ServiceLocator.GetService<LevelsService>();
            _progressionSettings = _configurationService.GetSettings<ProgressionSettings>();
            
        }
        
        public int ProgressiveScoresCount()
        {
            return _progressionSettings.WinScores +
                   _progressionSettings.ProgressiveWinScores * (_levelsService.CurrentLevel - 1);
        }

        public int ProgressiveLevelTime()
        {
            return _progressionSettings.LevelTime +
                   _progressionSettings.ProgressiveLevelTime * (_levelsService.CurrentLevel - 1);
        }
        
        public int ProgressiveTargetsCount()
        {
            return _progressionSettings.TargetsCount +
                   _progressionSettings.ProgressiveTargetsCount * (_levelsService.CurrentLevel - 1);
        }
        
        public float ProgressiveShowTime()
        {
            return _progressionSettings.OpenedShowTime -
                   _progressionSettings.ProgressiveShowTime * (_levelsService.CurrentLevel - 1);
        }
    }
}