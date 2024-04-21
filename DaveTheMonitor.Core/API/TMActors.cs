using DaveTheMonitor.Core.Plugin;
using StudioForge.TotalMiner;

namespace DaveTheMonitor.Core.API
{
    /// <summary>
    /// Provides easy access to all vanilla actors as <see cref="CoreActor"/>s.
    /// </summary>
    public static class TMActors
    {
        public static CoreActor None => ActorRegister.GetActor(ActorType.None);
        public static CoreActor Player => ActorRegister.GetActor(ActorType.Player);
        public static CoreActor Duck => ActorRegister.GetActor(ActorType.Duck);
        public static CoreActor AyrshireCow => ActorRegister.GetActor(ActorType.AyrshireCow);
        public static CoreActor Sheep => ActorRegister.GetActor(ActorType.Sheep);
        public static CoreActor Spider => ActorRegister.GetActor(ActorType.Spider);
        public static CoreActor Dryad => ActorRegister.GetActor(ActorType.Dryad);
        public static CoreActor Skeleton => ActorRegister.GetActor(ActorType.Skeleton);
        public static CoreActor Djinn => ActorRegister.GetActor(ActorType.Djinn);
        public static CoreActor Goblin => ActorRegister.GetActor(ActorType.Goblin);
        public static CoreActor Orc => ActorRegister.GetActor(ActorType.Orc);
        public static CoreActor TrollChief => ActorRegister.GetActor(ActorType.TrollChief);
        public static CoreActor TrollBoy => ActorRegister.GetActor(ActorType.TrollBoy);
        public static CoreActor TrollGirl => ActorRegister.GetActor(ActorType.TrollGirl);
        public static CoreActor Werewolf => ActorRegister.GetActor(ActorType.Werewolf);
        public static CoreActor HighlandCow => ActorRegister.GetActor(ActorType.HighlandCow);
        public static CoreActor Alpaca => ActorRegister.GetActor(ActorType.Alpaca);
        public static CoreActor HermesWraith => ActorRegister.GetActor(ActorType.HermesWraith);
        public static CoreActor Boy => ActorRegister.GetActor(ActorType.Boy);
        public static CoreActor Girl => ActorRegister.GetActor(ActorType.Girl);
        public static CoreActor Original => ActorRegister.GetActor(ActorType.Original);
        public static CoreActor Angel => ActorRegister.GetActor(ActorType.Angel);
        public static CoreActor Carpenter => ActorRegister.GetActor(ActorType.Carpenter);
        public static CoreActor Caveman => ActorRegister.GetActor(ActorType.Caveman);
        public static CoreActor Chef => ActorRegister.GetActor(ActorType.Chef);
        public static CoreActor Cowboy => ActorRegister.GetActor(ActorType.Cowboy);
        public static CoreActor Diablo => ActorRegister.GetActor(ActorType.Diablo);
        public static CoreActor Explorer => ActorRegister.GetActor(ActorType.Explorer);
        public static CoreActor Entrepreneur => ActorRegister.GetActor(ActorType.Entrepreneur);
        public static CoreActor GoldenKnight => ActorRegister.GetActor(ActorType.GoldenKnight);
        public static CoreActor Handyman => ActorRegister.GetActor(ActorType.Handyman);
        public static CoreActor Hippie => ActorRegister.GetActor(ActorType.Hippie);
        public static CoreActor Indian => ActorRegister.GetActor(ActorType.Indian);
        public static CoreActor InvaderMan => ActorRegister.GetActor(ActorType.InvaderMan);
        public static CoreActor Jamaican => ActorRegister.GetActor(ActorType.Jamaican);
        public static CoreActor Knight => ActorRegister.GetActor(ActorType.Knight);
        public static CoreActor Lumberjack => ActorRegister.GetActor(ActorType.Lumberjack);
        public static CoreActor Madman => ActorRegister.GetActor(ActorType.Madman);
        public static CoreActor Medic => ActorRegister.GetActor(ActorType.Medic);
        public static CoreActor Ninja => ActorRegister.GetActor(ActorType.Ninja);
        public static CoreActor Pirate => ActorRegister.GetActor(ActorType.Pirate);
        public static CoreActor Prisoner => ActorRegister.GetActor(ActorType.Prisoner);
        public static CoreActor Pupil => ActorRegister.GetActor(ActorType.Pupil);
        public static CoreActor Refugee => ActorRegister.GetActor(ActorType.Refugee);
        public static CoreActor Robotic => ActorRegister.GetActor(ActorType.Robotic);
        public static CoreActor Sage => ActorRegister.GetActor(ActorType.Sage);
        public static CoreActor Soldier => ActorRegister.GetActor(ActorType.Soldier);
        public static CoreActor Terminator => ActorRegister.GetActor(ActorType.Terminator);
        public static CoreActor TreeHugger => ActorRegister.GetActor(ActorType.TreeHugger);
        public static CoreActor Zeus => ActorRegister.GetActor(ActorType.Zeus);
        public static CoreActor Alien => ActorRegister.GetActor(ActorType.Alien);
        public static CoreActor Bomberman => ActorRegister.GetActor(ActorType.Bomberman);
        public static CoreActor ElfBoy => ActorRegister.GetActor(ActorType.ElfBoy);
        public static CoreActor ElfGirl => ActorRegister.GetActor(ActorType.ElfGirl);
        public static CoreActor Farmer => ActorRegister.GetActor(ActorType.Farmer);
        public static CoreActor Guard => ActorRegister.GetActor(ActorType.Guard);
        public static CoreActor Hobo => ActorRegister.GetActor(ActorType.Hobo);
        public static CoreActor Templar => ActorRegister.GetActor(ActorType.Templar);
        public static CoreActor Zombie => ActorRegister.GetActor(ActorType.Zombie);
        public static CoreActor Dwarf1 => ActorRegister.GetActor(ActorType.Dwarf1);
        public static CoreActor Dwarf2 => ActorRegister.GetActor(ActorType.Dwarf2);
        public static CoreActor Dwarf3 => ActorRegister.GetActor(ActorType.Dwarf3);
        public static CoreActor Dwarf4 => ActorRegister.GetActor(ActorType.Dwarf4);
        public static CoreActor Dwarf5 => ActorRegister.GetActor(ActorType.Dwarf5);
        public static CoreActor Dwarf6 => ActorRegister.GetActor(ActorType.Dwarf6);
        public static CoreActor Girl2 => ActorRegister.GetActor(ActorType.Girl2);
        public static CoreActor Girl3 => ActorRegister.GetActor(ActorType.Girl3);
        public static CoreActor Girl4 => ActorRegister.GetActor(ActorType.Girl4);
        public static CoreActor Princess => ActorRegister.GetActor(ActorType.Princess);
        public static CoreActor King => ActorRegister.GetActor(ActorType.King);
        public static CoreActor Astronaut => ActorRegister.GetActor(ActorType.Astronaut);
        public static CoreActor DemiGod => ActorRegister.GetActor(ActorType.DemiGod);
        public static CoreActor DemiGoddess => ActorRegister.GetActor(ActorType.DemiGoddess);
        public static CoreActor Sailor => ActorRegister.GetActor(ActorType.Sailor);
        public static CoreActor Fisherman => ActorRegister.GetActor(ActorType.Fisherman);
        public static CoreActor Policeman => ActorRegister.GetActor(ActorType.Policeman);
        public static CoreActor TesterMan => ActorRegister.GetActor(ActorType.TesterMan);
        public static CoreActor Self => ActorRegister.GetActor(ActorType.Self);
        private static ICoreActorRegistry ActorRegister => CorePlugin.Instance.Game.ActorRegistry;
    }
}
