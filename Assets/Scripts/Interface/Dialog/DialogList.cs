using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogList : MonoBehaviour
{
    [SerializeField]
    public Dictionary<string, Line> dialogLines = new Dictionary<string, Line>();
    public QuestList questDatabase;

    void Awake()
    {
        questDatabase = GameObject.FindWithTag("Database").GetComponent<QuestList>();
    }

    public bool ContainsLine(string key)
    {
        return dialogLines.ContainsKey(key);
    }

    public Line GetLine(string key)
    {
        Line value;

        dialogLines.TryGetValue(key, out value);

        return value;
    }

    public Dictionary<string, Line> GetDialogLines(GameObject npc)
    {
        AddLines(npc);
        return dialogLines;

    }

    public void AddLines(GameObject partner)
    {
        ClearLines();
        /*
        switch (partner.name)
        {
            case "Merchant":
                {
                    dialogLines.Add("S", new Line(new LinePart[] { new LinePart(false, "guten tag fremder was verschlägt dich in diese gegend") }, new string[] { "A", "B", "C" }));
                    dialogLines.Add("A", new Line("handeln", new LinePart[] { new LinePart(true, "ich habe gehört du verkaufst gute waren"), new LinePart(false, "am besten überzeugst du dich selbst davon") }, LineType.Trade, new string[] { "A", "B", "C" }));
                    dialogLines.Add("B", new Line("infos", new LinePart[] { new LinePart(true, "wass muss ich über die gegend wissen?"), new LinePart(false, "naja im moment gibt es hier nicht viel frag mal die entwickler ob hier noch was kommt") }, new string[] { "A", "B", "C" }));
                    dialogLines.Add("C", new Line("ende", new LinePart[] { new LinePart(true, "ich glaub ich hab mich verlaufen (ende)"), new LinePart(false, "auf wiedersehen") }));

                    /*
                    lines.Add(new Line("Q", "Hello"));
                    lines.Add(new Line("A", "Hello!? where are you?", LineType.Continue, 3));
                    lines.Add(new Line("B", "Hello, what do you sell?", LineType.ActionContinue, Line.LineAction.Open, 3));
                    lines.Add(new Line("C", "Who is talking to me", LineType.Continue, 3));
                    lines.Add(new Line("AQ", "Down here!"));
                    lines.Add(new Line("AA", "I can't see you", LineType.Continue, 1));
                    lines.Add(new Line("AB", "I don't have time for bad jokes", LineType.Continue, 1));
                    lines.Add(new Line("AC", "Can i help you?", LineType.Continue, 3));
                    lines.Add(new Line("BQ", "See you"));
                    lines.Add(new Line("BA", "Bye", LineType.Finish));
                    lines.Add(new Line("BB", "Wait i forgot sth", LineType.ActionContinue, Line.LineAction.Open, 1));
                    lines.Add(new Line("BC", "See you", LineType.Finish));
                    lines.Add(new Line("CQ", "A Ghost whoooo"));
                    lines.Add(new Line("CA", "Ahhh", LineType.Finish));
                    lines.Add(new Line("CB", "not funny", LineType.Continue, 1));
                    lines.Add(new Line("CC", "nice try", LineType.Continue, 1));
                    lines.Add(new Line("AAQ", "Give you some shit"));
                    lines.Add(new Line("AAA", "thx bye", LineType.ActionFinish, Line.LineAction.Add));
                    lines.Add(new Line("ABQ", "Hmm"));
                    lines.Add(new Line("ABA", "bye", LineType.Finish));
                    lines.Add(new Line("BBQ", "Hmm"));
                    lines.Add(new Line("BBA", "bye", LineType.Finish));
                    lines.Add(new Line("CBQ", "Hmm"));
                    lines.Add(new Line("CBA", "bye", LineType.Finish));
                    lines.Add(new Line("CCQ", "Hmm"));
                    lines.Add(new Line("CCA", "bye", LineType.Finish));
                    lines.Add(new Line("ACQ", "Yes, please kill the evil Cube"));
                    lines.Add(new Line("ACA", "Ok see you", LineType.Finish));
                    lines.Add(new Line("ACB", "No thank you", LineType.Finish));
                    lines.Add(new Line("ACC", "How can I do this?", LineType.Continue, 1));
                    lines.Add(new Line("ACCQ", "I have these arrows for you have fun"));
                    lines.Add(new Line("ACCA", "Thank you, see you soon", LineType.ActionFinish, Line.LineAction.Add));*/
                    /*break;
                }
            case "Guard":
                {
                    dialogLines.Add("S", new Line(new LinePart[] { new LinePart(false, "STOP") }, new string[] { "A" }));
                    dialogLines.Add("A", new Line("offensiv", new LinePart[] { new LinePart(true, "ich hau dir auf die fresse"), new LinePart(false, "versuchs doch") }));

                    break;
                }
            case "Trainer":
                {
                    dialogLines.Add("S", new Line(new LinePart[] { new LinePart(false, "are you here to upgrade your combat skills?") }, new string[] { "A", "B", "C" }));
                    dialogLines.Add("A", new Line("melee", new LinePart[] { new LinePart(true, "Show me some new melee combat moves") }));
                    dialogLines.Add("B", new Line("ranged", new LinePart[] { new LinePart(true, "Show me some new ranged combat tricks") }));
                    dialogLines.Add("C", new Line("(end)", new LinePart[] { new LinePart(true, "No maybe later"), new LinePart(false, "Your welcome") }));
                    break;
                }
            case "Freelancer":
                {
                    dialogLines.Add("S", new Line(new LinePart[] { new LinePart(false, "how can i help you?") }, new string[] { "A", "B" }));
                    dialogLines.Add("A", new Line("Join me", new LinePart[] { new LinePart(true, "Follow me") }, LineType.JoinGroup/*, new TreeChange[] { new InsertLine("C", "S"), new InsertLine("E", "S"), new RemoveLine("A") }*//*));
                    dialogLines.Add("B", new Line("(end)", new LinePart[] { new LinePart(true, "i'll return later"), new LinePart(false, "you know where to find me") }));

                    dialogLines.Add("C", new Line("Wait", new LinePart[] { new LinePart(true, "wait her") }, LineType.WaitGroup));
                    dialogLines.Add("D", new Line("Follow", new LinePart[] { new LinePart(true, "Follow me") }, LineType.FollowGroup));
                    dialogLines.Add("E", new Line("Send home", new LinePart[] { new LinePart(true, "go home") }, LineType.LeaveGroup));
                    break;
                }*/
            /*
        case "ArenaManager":
            {
                // Quest:"Arena Champion" isn't started
                if (questDatabase.quests[0].status == Quest.Status.Available)
                {
                    lines.Add(new Line("Q", "Hello would you like to fight in the arena", new string[] { "A", "B", "C" }));
                    lines.Add(new Line("A", "Of course tell me more", "AQ"));
                    lines.Add(new Line("AQ", "It's simple defeat all fighters and you'll be the champion of the arena", new string[] { "AA", "AB", "C" }));
                    lines.Add(new Line("AA", "Are there more rules i need to know?", "AAQ"));
                    lines.Add(new LineQuest("AB", "Who is the first fighter i have o beat", "ABQ", LineType.QuestStatus, questDatabase.quests[0], Quest.Status.Active));
                    lines.Add(new Line("ABQ", "Talk to the guy behind the arena he is a good start for you", new string[] { "B", "C" }));
                }
                // Quest:"Arena Champion" is started ...
                else if (questDatabase.quests[0].status == Quest.Status.Active)
                {
                    int notFinished = -1;
                    for (int i = 0; i < 3; i++)
                    {
                        if (!questDatabase.quests[0].parts[i].finished)
                        {
                            notFinished = i;
                            break;
                        }
                    }
                    Debug.Log(notFinished);
                    // ... and all fighters are defeated
                    if (notFinished == -1)
                    {
                        lines.Add(new Line("Q", "Had you defeated all fighters?", new string[] { "A", "B", "C" }));
                        lines.Add(new LineQuest("A", "Yes they were easy oponnents", "AQ", LineType.QuestContinue, questDatabase.quests[0], 3));
                        lines.Add(new Line("AQ", "Good work you are now the champion of the arena", new string[] { "B", "AC" }));
                        lines.Add(new Line("AC", "Thanks bye (End)", LineType.Finish));
                    }
                    // ... but not all fighters are defeated
                    else
                    {
                        lines.Add(new Line("Q", "Had you defeated all fighters?", new string[] { "A", "B", "C" }));
                        lines.Add(new Line("A", "No who is my next opponent", "AQ"));
                        lines.Add(new Line("D", "Can you tell me the rules again?", "AAQ"));
                        lines.Add(new Line("AQ", "Go to fighter " + notFinished, new string[] { "D", "B", "C" }));
                    }
                }
                // Quest:"Arena Champion" is finished
                else
                {
                    lines.Add(new Line("Q", "Hello Champion may i help you", new string[] { "B", "C" }));
                }
                // Allways load
                lines.Add(new Line("AAQ", " 1. Never leave the arena while fighting \n 2. if you kill your oponnent you will become problems \n 3. the match starts when all fighters have entered the arena", new string[] { "B", "C" }));
                lines.Add(new Line("B", "I heard you sell strong items", "BQ", LineType.Open));
                lines.Add(new Line("C", "(End)", LineType.Finish));
                break;
            }
        case "Gladiator":
            {
                // Quest:"Arena Champion" isn't started
                if (questDatabase.quests[0].status == Quest.Status.Available)
                {
                    lines.Add(new Line("Q", "Are you interested in a fight in the arena?", new string[] { "A", "B", "C" }));
                    lines.Add(new Line("A", "Yes why not", "AQ"));
                    lines.Add(new Line("C", "No not yet (End)", LineType.Finish));
                    lines.Add(new Line("AQ", "Talk to the arena manager at the entrance of the arena", new string[] { "B", "AC" }));
                    lines.Add(new Line("AC", "Thanks bye (End)", LineType.Finish));

                }
                // Quest:"Arena Champion" is started ...
                else if (questDatabase.quests[0].status == Quest.Status.Active)
                {
                    // ... but not fighted against this gladiator
                    if (!questDatabase.quests[0].parts[0].finished)
                    {
                        lines.Add(new Line("Q", "Hey are you here to fight?", new string[] { "A", "B", "C" }));
                        lines.Add(new LineArena("A", "Yes let's go", LineType.Arena, GameObject.FindWithTag("Arena"), questDatabase.quests[0], 0));
                        lines.Add(new Line("C", "No not yet (End)", LineType.Finish));
                    }
                    // ... and this fighter is allready defeated
                    else
                    {
                        lines.Add(new Line("Q", "Never thought you'll beat me", new string[] { "B", "C" }));
                        lines.Add(new Line("C", "Bye (End)", LineType.Finish));
                    }
                }
                // Quest:"Arena Champion" is finished
                else
                {
                    lines.Add(new Line("Q", "Hey whats up?", new string[] { "B", "C" }));
                    lines.Add(new Line("C", "Fine thanks (End)", LineType.Finish));
                }
                // Always load
                lines.Add(new Line("B", "can you teach me in onehanded combat?", "BQ", LineType.Open));

                break;
            }
        case "Freelancer":
            {
                // is in group
                if (GameObject.FindWithTag("Player").GetComponent<PlayerAttributes>().group == partner.GetComponent<PlayerAttributes>().group)
                {
                    lines.Add(new Line("Q", "Whats up", new string[] { "A", "B", "C" , "D" }));

                    // waiting
                    if(partner.GetComponent<EnemyAI>().externalDestination == partner.transform.position)
                    {
                        lines.Add(new LineGroup("A", "Follow me", LineType.Group, LineGroup.GroupAction.Follow));
                    }
                    // follows
                    else
                    {
                        lines.Add(new LineGroup("A", "Wait here", LineType.Group, LineGroup.GroupAction.Wait));
                    }

                    lines.Add(new LineGroup("C", "Go home", LineType.Group, LineGroup.GroupAction.Leave));
                    lines.Add(new Line("D", "(End)", LineType.Finish));
                }
                // is not in group
                else
                {
                    lines.Add(new Line("Q", "do you need help form an experienced witcher?", new string[] { "A", "B", "C" }));
                    lines.Add(new LineGroup("A", "Follow me", LineType.Group, LineGroup.GroupAction.Join));
                    lines.Add(new Line("C", "Not yet (End)", LineType.Finish));
                }
                // Always load
                lines.Add(new Line("B", "Can you teach me some of your skills?", LineType.Finish));

                break;
            }*/
            /*default:
                {
                    dialogLines.Add("S", new Line("S", new LinePart[] { new LinePart(false, "leave me alone"), new LinePart(true, "ok i will return later") }));

                    break;
                }

        }*/
    }

    public void ClearLines()
    {
        dialogLines.Clear();
    }
}
