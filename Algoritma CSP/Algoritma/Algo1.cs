using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TubesAI.Model;
using System.Windows.Forms;

namespace Algoritma
{
    public class BFS : AgentInterface
    {
        public BFS()
        {
        }

        public bool firstTurn
        {
            get
            {
                return firstTurn;
            }

            set
            {
                firstTurn = value;
            }
        }

        public List<ElemenAksi> Execute(Team myTeam, Team enTeam)
        {
            List<ElemenAksi> output = new List<ElemenAksi>(11);
            foreach (var unit in myTeam.listUnit)
            {
                Unit at_unit = enTeam.listUnit.First();
                foreach (var enemy_unit in enTeam.listUnit)
                {
                    if (at_unit.isDead())
                        at_unit = enemy_unit;
                    if (at_unit.getCurrentHP() > enemy_unit.getCurrentHP() && !enemy_unit.isDead())
                        at_unit = enemy_unit;
                    //enemy_unit.
                }
                output.Add(unit.Attack(at_unit.index, enTeam));
                //else output.Add(unit.useItem(2, myTeam, Item.potion));
            }
            return output;
        }
    }

    public class DFS : AgentInterface
    {
        public DFS()
        {
        }

        public bool firstTurn
        {
            get
            {
                return firstTurn;
            }

            set
            {
                firstTurn = value;
            }
        }

        public List<ElemenAksi> Execute(Team myTeam, Team enTeam)
        {
            List<ElemenAksi> output = new List<ElemenAksi>(11);
            return output;
        }
    }

    public class UCS : AgentInterface
    {
        public UCS()
        {
        }

        public bool firstTurn
        {
            get
            {
                return firstTurn;
            }

            set
            {
                firstTurn = value;
            }
        }

        public List<ElemenAksi> Execute(Team myTeam, Team enTeam)
        {
            List<ElemenAksi> output = new List<ElemenAksi>(11);
            return output;
        }
    }

    public class Greedy : AgentInterface
    {
        public Greedy()
        {
        }

        public bool firstTurn
        {
            get
            {
                return firstTurn;
            }

            set
            {
                firstTurn = value;
            }
        }

        public List<ElemenAksi> Execute(Team myTeam, Team enTeam)
        {
            List<ElemenAksi> output = new List<ElemenAksi>(11);
            return output;
        }
    }

    public class Astar : AgentInterface
    {
        public Astar()
        { 
        }

        public bool firstTurn
        {
            get
            {
                return firstTurn;
            }

            set
            {
                firstTurn = value;
            }
        }

        public List<ElemenAksi> Execute(Team myTeam, Team enTeam)
        {
            List<ElemenAksi> output = new List<ElemenAksi>(11);
            return output;
        }
    }

    public class CSP : AgentInterface
    {
        public CSP()
        {
        }

        public bool firstTurn
        {
            get
            {
                return firstTurn;
            }
            set
            {
                firstTurn = value;
            }
        }
                
        public List<ElemenAksi> Execute(Team myTeam, Team enTeam)
        {
            List<ElemenAksi> output = new List<ElemenAksi>(11);
            List<List<ElemenAksi>> temp2 = new List<List<ElemenAksi>>(11);
            int i;
            for (i = 0; i < 11; i++)
            {
                List<ElemenAksi> temp = new List<ElemenAksi>();
                temp2.Add(temp);
            }
            foreach (var myUnit in myTeam.listUnit)
            {
                if (!myUnit.isDead())
                {
                    if (myUnit is Medic)
                    {
                        foreach (var myunit in myTeam.listUnit)
                        {
                            temp2.ElementAt(myUnit.index).Add(myUnit.Heal(myunit.index, myTeam));
                        }
                        foreach (var myunit in myTeam.listUnit)
                        {
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myunit.index, myTeam, Item.life_potion));
                        }
                        foreach (var myunit in myTeam.listUnit)
                        {
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myunit.index, myTeam, Item.potion));
                        }
                    }
                    else if (myUnit is Swordsman)
                    {
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Spearman)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Medic)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Swordsman)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Rider)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var myunit in myTeam.listUnit)
                        {
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myunit.index, myTeam, Item.life_potion));
                        }
                        foreach (var myunit in myTeam.listUnit)
                        {
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myunit.index, myTeam, Item.potion));
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Archer)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                    }
                    else if (myUnit is Spearman)
                    {
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Rider)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Medic)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Spearman)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Archer)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var myunit in myTeam.listUnit)
                        {
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myunit.index, myTeam, Item.life_potion));
                        }
                        foreach (var myunit in myTeam.listUnit)
                        {
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myunit.index, myTeam, Item.potion));
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Swordsman)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                    }
                    else if (myUnit is Rider)
                    {
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Archer)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Medic)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Rider)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Swordsman)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Spearman)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                    }
                    else if (myUnit is Archer)
                    {
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Swordsman)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Medic)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Archer)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Spearman)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                        foreach (var myunit in myTeam.listUnit)
                        {
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myunit.index, myTeam, Item.life_potion));
                        }
                        foreach (var myunit in myTeam.listUnit)
                        {
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myunit.index, myTeam, Item.potion));
                        }
                        foreach (var enUnit in enTeam.listUnit)
                        {
                            if (enUnit is Rider)
                            {
                                temp2.ElementAt(myUnit.index).Add(myUnit.Attack(enUnit.index, enTeam));
                            }
                        }
                    }
                    temp2.ElementAt(myUnit.index).Add(myUnit.Defend());
                }
                }
            for (i = 0; i < 11; i++)
            {
                int a = 0;
                while (a < temp2.ElementAt(i).Count)
                {
                    if (temp2.ElementAt(i).ElementAt(a).aksi == Aksi.menyerang)
                    {
                        if (enTeam.listUnit.ElementAt(temp2.ElementAt(i).ElementAt(a).index_sasaran).isDead())
                        {
                            temp2.ElementAt(i).RemoveAt(a);
                        }
                        else if (enTeam.listUnit.ElementAt(temp2.ElementAt(i).ElementAt(a).index_sasaran).isBertahan)
                        {
                            temp2.ElementAt(i).RemoveAt(a);
                        }
                        else
                        {
                            a++;
                        }
                    }
                    else if (temp2.ElementAt(i).ElementAt(a).aksi == Aksi.heal)
                    {
                        if ((myTeam.listUnit.ElementAt(temp2.ElementAt(i).ElementAt(a).index_sasaran).isDead()) || (((myTeam.listUnit.ElementAt(temp2.ElementAt(i).ElementAt(a).index_sasaran).getMaxHP()) - (myTeam.listUnit.ElementAt(temp2.ElementAt(i).ElementAt(a).index_sasaran).getCurrentHP())) < 500))
                        {
                            temp2.ElementAt(i).RemoveAt(a);
                        }
                        else
                        {
                            a++;
                        }
                    }
                    else if (temp2.ElementAt(i).ElementAt(a).item == Item.potion)
                    {
                        if ((myTeam.listUnit.ElementAt(temp2.ElementAt(i).ElementAt(a).index_sasaran).isDead()) && ((myTeam.listUnit.ElementAt(temp2.ElementAt(i).ElementAt(a).index_sasaran).getMaxHP()) - (myTeam.listUnit.ElementAt(temp2.ElementAt(i).ElementAt(a).index_sasaran).getCurrentHP()) < 500))
                        {
                            temp2.ElementAt(i).RemoveAt(a);
                        }
                        else if (myTeam.isPotionRunOut())
                        {
                            temp2.ElementAt(i).RemoveAt(a);
                        }
                        else
                        {
                            a++;
                        }
                    }
                    else if (temp2.ElementAt(i).ElementAt(a).item == Item.life_potion)
                    {
                        if (!(myTeam.listUnit.ElementAt(temp2.ElementAt(i).ElementAt(a).index_sasaran).isDead()))
                        {
                            temp2.ElementAt(i).RemoveAt(a);
                        }
                        else
                        {
                            a++;
                        }
                    }
                    else
                    {
                        a++;
                    }
                }
            }

            for (i = 0; i < 11; i++)
            {
                try 
                {
                    output.Add(temp2.ElementAt(i).ElementAt(0));
                }
                catch
                {
                    output.Add(myTeam.listUnit.ElementAt(i).Defend());
                }
            }

            return output;
        }
    }
}
