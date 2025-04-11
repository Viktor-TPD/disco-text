using discotext.Models;
using discotext.Utils;

namespace discotext.Core
{
    public class Game
    {
        private Player _player;
        private bool _isRunning;
        private Dictionary<string, Location> _locations;
        private Dictionary<string, Item> _items;
        private CommandProcessor _commandProcessor;
        private GameText _gameText;
        private DialogueHandler _dialogueHandler;
        bool deathDisplayed = false;
        public Game()
        {
            _gameText = new GameText();
            _dialogueHandler = new DialogueHandler(_gameText, null);
            
            InitializeGameWorld();
            
            _commandProcessor = new CommandProcessor(this);
            
            _dialogueHandler = new DialogueHandler(_gameText, this);
            
            InitializeGameWorld();
            
            _isRunning = false;
        }
        
        private void InitializeGameWorld()
        {
            if (_commandProcessor == null)
            {
                _items = new ItemFactory(null, _gameText).CreateAllItems();
                _locations = new LocationFactory(_items).CreateLocations();
                _player = new Player(_locations["CenterOfRoom"]);
                return;
            }
            
            DialogueActionHandler exitDialogueHandler = () => _commandProcessor.ExitDialogueAfterEffect();
            DialogueActionHandler lookHandler = () => _commandProcessor.Look();
            
            _items = new ItemFactory(_player, _gameText, this, exitDialogueHandler, lookHandler).CreateAllItems();
            _locations = new LocationFactory(_items).CreateLocations();
            _player.CurrentLocation = _locations["CenterOfRoom"];
        }

        public void Start()
        {
            _isRunning = true;
            _gameText.DisplayIntro();
            _gameText.DisplayLocationDescription(_player.CurrentLocation);

            while (_isRunning)
            {
                Console.Write("> ");
                string input = Console.ReadLine().ToLower();

                if (input == "quit" || input == "exit" || _player.Health <= 0 || _player.Morale <= 0)
                {
                    _isRunning = false;
                    continue;
                }
                
                _commandProcessor.ProcessCommand(input);
                
                if (_player.Health <= 0)
                {
                     _gameText.DisplayHealthDeath();
                     deathDisplayed = true;
                     _isRunning = false;
                     continue;
                }
                if (_player.Morale <= 0)
                {
                    _gameText.DisplayMoraleDeath();
                    deathDisplayed = true;
                    _isRunning = false;
                    continue;
                }
            }
            _gameText.DisplayOutro();
        }

        public Player GetPlayer()
        {
            return _player;
        }

        public GameText GetGameText()
        {
            return _gameText;
        }
        
        public DialogueHandler GetDialogueHandler()
        {
            return _dialogueHandler;
        }

        public void EndGame()
        {
            _isRunning = false;
        }
    }
}