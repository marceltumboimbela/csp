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
        private int potion = 10;
        private int lifep = 10;

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
            if (!myTeam.isPotionRunOut() && potion <= 0) potion = 1;
            if (!myTeam.isLifePotionRunOut() && lifep <= 0) lifep = 1;
            int i;
            Boolean found = false;
            List<ElemenAksi> output = new List<ElemenAksi>(11);
            List<ElemenAksi> temp = new List<ElemenAksi>(11);
            Queue<List<ElemenAksi>> pohon = new Queue<List<ElemenAksi>>();
            int[] myHP = new int[11];
            int[] enHP = new int[11];
            int potn;
            int lifn;
            for (i = 0; i < 11; i++)
            {
                if (!(myTeam.listUnit.ElementAt(i).isDead()))
                {
                    myHP[i] = myTeam.listUnit.ElementAt(i).getCurrentHP();
                }
                else
                {
                    myHP[i] = 0;
                }
                if (!(enTeam.listUnit.ElementAt(i).isDead()))
                {
                    enHP[i] = enTeam.listUnit.ElementAt(i).getCurrentHP();
                }
                else
                {
                    enHP[i] = 0;
                }
            }
            int[] myHPn = new int[11];
            int[] enHPn = new int[11];

            pohon.Enqueue(output);
            while (pohon.Count() > 0 && !found)
            {
                output = pohon.Dequeue();
                myHP.CopyTo(myHPn, 0);
                enHP.CopyTo(enHPn, 0);
                potn = potion;
                lifn = lifep;

                foreach (ElemenAksi act in output)
                {
                    if ((act.aksi == Aksi.heal) || (act.aksi == Aksi.use_item && act.item == Item.potion))
                    {
                        myHPn[act.index_sasaran] += 500;
                        if (act.aksi == Aksi.use_item) potn -= 1;
                        if (myHPn[act.index_sasaran] > myTeam.listUnit.ElementAt(act.index_sasaran).getMaxHP())
                            myHPn[act.index_sasaran] = myTeam.listUnit.ElementAt(act.index_sasaran).getMaxHP();
                    }
                    if (act.aksi == Aksi.use_item && act.item == Item.life_potion)
                    {
                        myHPn[act.index_sasaran] = myTeam.listUnit.ElementAt(act.index_sasaran).getMaxHP() / 2;
                        lifn -= 1;
                    }
                    if (act.aksi == Aksi.menyerang)
                    {
                        enHPn[act.index_sasaran] -= CalculationDamage(myTeam.listUnit.ElementAt(act.index_pelaku), enTeam.listUnit.ElementAt(act.index_sasaran));
                    }
                }

                Unit unit;
                if (output.Count >= countMedic(myTeam))
                {
                    if (output.Count() - countMedic(myTeam) >= getFirstMedic(myTeam)) unit = myTeam.listUnit.ElementAt(output.Count);
                    else unit = myTeam.listUnit.ElementAt(output.Count() - countMedic(myTeam));

                }
                else unit = myTeam.listUnit.ElementAt(output.Count + getFirstMedic(myTeam));

                if (!(unit.isDead()))
                {
                    if (unit is Medic)
                    {
                        //heal
                        if (!((Medic)unit).isTidakBisaCuring())
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                if (found) break;
                                if (myHPn[j] > 0 && (myTeam.listUnit.ElementAt(j).getMaxHP() - myHPn.ElementAt(j)) >= 500 && !myTeam.listUnit.ElementAt(j).isDead())
                                {
                                    temp = new List<ElemenAksi>(11);
                                    copyoutput(output, temp);
                                    temp.Add(unit.Heal(myTeam.listUnit.ElementAt(j).index, myTeam));
                                    if (temp.Count() == 11)
                                        found = true;
                                    pohon.Enqueue(temp);
                                }
                            }
                        }
                    }
                    if (!(unit is Rider) && potn > 0)
                    {
                        //potion
                        for (int j = 0; j < 11; j++)
                        {
                            if (found) break;
                            if (myHPn.ElementAt(j) > 0 && (myTeam.listUnit.ElementAt(j).getMaxHP() - myHPn.ElementAt(j)) >= 500 && !myTeam.listUnit.ElementAt(j).isDead())
                            {
                                temp = new List<ElemenAksi>(11);
                                copyoutput(output, temp);
                                temp.Add(unit.useItem(myTeam.listUnit.ElementAt(j).index, myTeam, Item.potion));
                                potn--;
                                if (temp.Count() == 11)
                                    found = true;
                                pohon.Enqueue(temp);
                            }
                        }
                    }
                    if (!(unit is Rider) && lifep > 0)
                    {
                        //lpotion
                        for (int j = 0; j < 11; j++)
                        {
                            if (found) break;
                            if (myHPn.ElementAt(j) <= 0 && myTeam.listUnit.ElementAt(j).isDead())
                            {
                                temp = new List<ElemenAksi>(11);
                                copyoutput(output, temp);
                                temp.Add(unit.useItem(myTeam.listUnit.ElementAt(j).index, myTeam, Item.life_potion));
                                lifn--;
                                if (temp.Count() == 11)
                                    found = true;
                                pohon.Enqueue(temp);
                            }
                        }
                    }
                    if (!(unit is Medic))
                    {
                        //attack
                        for (int j = 0; j < 11; j++)
                        {
                            if (found) break;
                            if (enHPn.ElementAt(j) > 0)
                            {
                                temp = new List<ElemenAksi>(11);
                                copyoutput(output, temp);
                                temp.Add(unit.Attack(enTeam.listUnit.ElementAt(j).index, enTeam));
                                if (temp.Count() == 11)
                                    found = true;
                                pohon.Enqueue(temp);
                            }
                        }
                    }
                }
                if (!found)
                {
                    temp = new List<ElemenAksi>(11);
                    copyoutput(output, temp);
                    temp.Add(unit.Defend());
                    if (temp.Count() == 11)
                        found = true;
                    pohon.Enqueue(temp);
                }
            }
            foreach (ElemenAksi act in temp)
            {
                if (act.aksi == Aksi.use_item && act.item == Item.potion) potion--;
                if (act.aksi == Aksi.use_item && act.item == Item.life_potion) lifep--;
            }
            return temp;
        }

        private void copyoutput(List<ElemenAksi> output, List<ElemenAksi> temp)
        {
            foreach (ElemenAksi act in output)
            {
                temp.Add(act);
            }
        }

        private int CalculationDamage(Unit satu, Unit dua)
        {
            int DamageTaken = 200;

            if (satu is Archer)
            {
                if (dua is Rider)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Swordsman) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }
            else if (satu is Swordsman)
            {
                if (dua is Archer)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Spearman) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }
            else if (satu is Spearman)
            {
                if (dua is Swordsman)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Rider) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }
            else if (satu is Rider)
            {
                if (dua is Spearman)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Archer) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }

            return DamageTaken;
        }

        private int getFirstMedic(Team myTeam)
        {
            foreach (var unit in myTeam.listUnit)
            {
                if (unit is Medic) return unit.index;
            }
            return 0;
        }

        private int countMedic(Team myTeam)
        {
            int c = 0;
            foreach (var unit in myTeam.listUnit)
            {
                if (unit is Medic) c++;
            }
            return c;
        }
    }

    public class DFS : AgentInterface
    {
        int potion = 10;
        int lifep = 10;
        
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

        /*Brute Force
        public List<ElemenAksi> Execute(Team myTeam, Team enTeam)
        {
            List<ElemenAksi> output = new List<ElemenAksi>(11);
            int i = 0;
            foreach (var unit in myTeam.listUnit)
            {
                foreach (var eunit in enTeam.listUnit)
                {
                    if (!eunit.isDead())
                    {
                        i = eunit.index;
                        break;
                    }
                }
                output.Add(unit.Attack(enTeam.listUnit.ElementAt(i).index, enTeam));
            }
            return output;
        }*/
        //* DFS Filman
         
        public List<ElemenAksi> Execute(Team myTeam, Team enTeam)
        {
            if (!myTeam.isPotionRunOut() && potion <= 0) potion = 1;
            if (!myTeam.isLifePotionRunOut() && lifep <= 0) lifep = 1;
            int i;
            Boolean found = false;
            List<ElemenAksi> output = new List<ElemenAksi>(11);
            List<ElemenAksi> temp = new List<ElemenAksi>(11);
            Stack<List<ElemenAksi>> pohon = new Stack<List<ElemenAksi>>();
            int[] myHP = new int[11];
            int[] enHP = new int[11];
            int potn;
            int lifn;
            for (i = 0; i < 11; i++)
            {
                if (!(myTeam.listUnit.ElementAt(i).isDead()))
                {
                    myHP[i] = myTeam.listUnit.ElementAt(i).getCurrentHP();
                }
                else
                {
                    myHP[i] = 0;
                }
                if (!(enTeam.listUnit.ElementAt(i).isDead()))
                {
                    enHP[i] = enTeam.listUnit.ElementAt(i).getCurrentHP();
                }
                else
                {
                    enHP[i] = 0;
                }
            }
            int[] myHPn = new int[11];
            int[] enHPn = new int[11];

            pohon.Push(output);
            while (pohon.Count() > 0 && !found)
            {
                output = pohon.Pop();
                myHP.CopyTo(myHPn, 0);
                enHP.CopyTo(enHPn, 0);
                potn = potion;
                lifn = lifep;

                foreach (ElemenAksi act in output)
                {
                    if ((act.aksi == Aksi.heal) || (act.aksi == Aksi.use_item && act.item == Item.potion))
                    {
                        myHPn[act.index_sasaran] += 500;
                        if (act.aksi == Aksi.use_item) potn -= 1;
                        if (myHPn[act.index_sasaran] > myTeam.listUnit.ElementAt(act.index_sasaran).getMaxHP())
                            myHPn[act.index_sasaran] = myTeam.listUnit.ElementAt(act.index_sasaran).getMaxHP();
                    }
                    if (act.aksi == Aksi.use_item && act.item == Item.life_potion)
                    {
                        myHPn[act.index_sasaran] = myTeam.listUnit.ElementAt(act.index_sasaran).getMaxHP() / 2;
                        lifn -= 1;
                    }
                    if (act.aksi == Aksi.menyerang)
                    {
                        enHPn[act.index_sasaran] -= CalculationDamage(myTeam.listUnit.ElementAt(act.index_pelaku), enTeam.listUnit.ElementAt(act.index_sasaran));
                    }
                }

                Unit unit;
                if (output.Count >= countMedic(myTeam))
                {
                    if (output.Count() - countMedic(myTeam) >= getFirstMedic(myTeam)) unit = myTeam.listUnit.ElementAt(output.Count);
                    else unit = myTeam.listUnit.ElementAt(output.Count() - countMedic(myTeam));

                }
                else unit = myTeam.listUnit.ElementAt(output.Count + getFirstMedic(myTeam));

                if (!found)
                {
                    temp = new List<ElemenAksi>(11);
                    copyoutput(output, temp);
                    temp.Add(unit.Defend());
                    if (temp.Count() == 11)
                        found = true;
                    pohon.Push(temp);
                }
                if (!(unit.isDead()))
                {
                    if (!(unit is Medic))
                    {
                        //attack
                        for (int j = 0; j < 11; j++)
                        {
                            if (found) break;
                            if (enHPn.ElementAt(j) > 0)
                            {
                                temp = new List<ElemenAksi>(11);
                                copyoutput(output, temp);
                                temp.Add(unit.Attack(enTeam.listUnit.ElementAt(j).index, enTeam));
                                if (temp.Count() == 11)
                                    found = true;
                                pohon.Push(temp);
                            }
                        }
                    }
                    
                    if (!(unit is Rider) && lifep > 0)
                    {
                        //lpotion
                        for (int j = 0; j < 11; j++)
                        {
                            if (found) break;
                            if (myHPn.ElementAt(j) <= 0 && myTeam.listUnit.ElementAt(j).isDead())
                            {
                                temp = new List<ElemenAksi>(11);
                                copyoutput(output, temp);
                                temp.Add(unit.useItem(myTeam.listUnit.ElementAt(j).index, myTeam, Item.life_potion));
                                lifn--;
                                if (temp.Count() == 11)
                                    found = true;
                                pohon.Push(temp);
                            }
                        }
                    }

                    if (!(unit is Rider) && potn > 0)
                    {
                        //potion
                        for (int j = 0; j < 11; j++)
                        {
                            if (found) break;
                            if (myHPn.ElementAt(j) > 0 && (myTeam.listUnit.ElementAt(j).getMaxHP() - myHPn.ElementAt(j)) >= 500 && !myTeam.listUnit.ElementAt(j).isDead())
                            {
                                temp = new List<ElemenAksi>(11);
                                copyoutput(output, temp);
                                temp.Add(unit.useItem(myTeam.listUnit.ElementAt(j).index, myTeam, Item.potion));
                                potn--;
                                if (temp.Count() == 11)
                                    found = true;
                                pohon.Push(temp);
                            }
                        }
                    }
                    
                    if (unit is Medic)
                    {
                        //heal
                        if (!((Medic)unit).isTidakBisaCuring())
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                if (found) break;
                                if (myHPn[j] > 0 && (myTeam.listUnit.ElementAt(j).getMaxHP() - myHPn.ElementAt(j)) >= 500 && !myTeam.listUnit.ElementAt(j).isDead())
                                {
                                    temp = new List<ElemenAksi>(11);
                                    copyoutput(output, temp);
                                    temp.Add(unit.Heal(myTeam.listUnit.ElementAt(j).index, myTeam));
                                    if (temp.Count() == 11)
                                        found = true;
                                    pohon.Push(temp);
                                }
                            }
                        }
                    }
                }
                
            }
            foreach (ElemenAksi act in temp)
            {
                if (act.aksi == Aksi.use_item && act.item == Item.potion) potion--;
                if (act.aksi == Aksi.use_item && act.item == Item.life_potion) lifep--;
            }
            //semiLog(temp, myTeam, enTeam);
            return temp;
        }

        private void copyoutput(List<ElemenAksi> output, List<ElemenAksi> temp)
        {
            foreach (ElemenAksi act in output)
            {
                temp.Add(act);
            }
        }

        private int CalculationDamage(Unit satu, Unit dua)
        {
            int DamageTaken = 200;

            if (satu is Archer)
            {
                if (dua is Rider)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Swordsman) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }
            else if (satu is Swordsman)
            {
                if (dua is Archer)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Spearman) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }
            else if (satu is Spearman)
            {
                if (dua is Swordsman)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Rider) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }
            else if (satu is Rider)
            {
                if (dua is Spearman)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Archer) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }

            return DamageTaken;
        }

        private int getFirstMedic(Team myTeam)
        {
            foreach (var unit in myTeam.listUnit)
            {
                if (unit is Medic) return unit.index;
            }
            return 0;
        }

        private int countMedic(Team myTeam)
        {
            int c = 0;
            foreach (var unit in myTeam.listUnit)
            {
                if (unit is Medic) c++;
            }
            return c;
        }
        private void semiLog(List<ElemenAksi> LA, Team myTeam, Team enTeam)
        {
            String S = "";
            String temp = "";
            S = "action:\n";
            foreach (var act in LA)
            {
                if (act.aksi == Aksi.bertahan) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Bertahan";
                else if (act.aksi == Aksi.menyerang) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Menyerang " + getJob(enTeam, act.index_sasaran) + getJobIndex(enTeam, act.index_sasaran);
                else if (act.aksi == Aksi.heal) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Meng-Heal " + getJob(myTeam, act.index_sasaran) + getJobIndex(myTeam, act.index_sasaran);
                else if (act.aksi == Aksi.use_item && act.item == Item.potion) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Memakai Potion kepada  " + getJob(myTeam, act.index_sasaran) + getJobIndex(myTeam, act.index_sasaran);
                else if (act.aksi == Aksi.use_item && act.item == Item.life_potion) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Memakai Life-Potion ke " + getJob(myTeam, act.index_sasaran) + getJobIndex(myTeam, act.index_sasaran);
                S += (temp + "\n");
            }
            S += "\nmyTeam:\n";
            foreach (var unit in myTeam.listUnit)
            {
                S += (unit.getCurrentHP() + "\t" + getJob(myTeam, unit.index) + getJobIndex(myTeam, unit.index) + "\n");
            }
            S += "\nenTeam:\n";
            foreach (var unit in enTeam.listUnit)
            {
                S += (unit.getCurrentHP() + "\t" + getJob(enTeam, unit.index) + getJobIndex(enTeam, unit.index) + "\n");
            }
            MessageBox.Show(S);
        }
        private String getJob(Team myTeam, int index)
        {
            Unit unit = myTeam.listUnit.ElementAt(index);
            if (unit is Archer) return "Archer";
            else if (unit is Swordsman) return "Swordsman";
            else if (unit is Spearman) return "Spearman";
            else if (unit is Rider) return "Rider";
            else if (unit is Medic) return "Medic";
            return "error";
        }
        private int getJobIndex(Team myTeam, int index)
        {
            String Job = getJob(myTeam, index);
            int count = 0;
            foreach (var unit in myTeam.listUnit)
            {
                if (Job == getJob(myTeam, unit.index))
                {
                    if (unit.index != index) count++;
                    else break;
                }

            }
            return count;
        }

         //*/
    }

    public class UCS : AgentInterface
    {
        private int potion = 10;
        private int lifep = 10;

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
            if (!myTeam.isPotionRunOut() && potion <= 0) potion = 1;
            if (!myTeam.isLifePotionRunOut() && lifep <= 0) lifep = 1;
            int i;
            Boolean found = false;
            List<ElemenAksi> output = new List<ElemenAksi>(11);
            List<ElemenAksi> temp = new List<ElemenAksi>(11);
            List<List<ElemenAksi>> pohon = new List<List<ElemenAksi>>();
            List<int> cost = new List<int>();
            int[] myHP = new int[11];
            int[] enHP = new int[11];
            int potn;
            int lifn;
            for (i = 0; i < 11; i++)
            {
                if (!(myTeam.listUnit.ElementAt(i).isDead()))
                {
                    myHP[i] = myTeam.listUnit.ElementAt(i).getCurrentHP();
                }
                else
                {
                    myHP[i] = 0;
                }
                if (!(enTeam.listUnit.ElementAt(i).isDead()))
                {
                    enHP[i] = enTeam.listUnit.ElementAt(i).getCurrentHP();
                }
                else
                {
                    enHP[i] = 0;
                }
            }
            int[] myHPn = new int[11];
            int[] enHPn = new int[11];

            cost.Add(myHP.Sum() - enHP.Sum());
            pohon.Add(output);
            while (pohon.Count() > 0 && !found)
            {
                i = cost.Max();
                output = pohon.ElementAt(cost.IndexOf(i));
                pohon.RemoveAt(cost.IndexOf(i));
                cost.RemoveAt(cost.IndexOf(i));
                myHP.CopyTo(myHPn, 0);
                enHP.CopyTo(enHPn, 0);
                potn = potion;
                lifn = lifep;

                foreach (ElemenAksi act in output)
                {
                    if ((act.aksi == Aksi.heal) || (act.aksi == Aksi.use_item && act.item == Item.potion))
                    {
                        myHPn[act.index_sasaran] += 500;
                        if(act.aksi == Aksi.use_item)potn -= 1;
                        if (myHPn[act.index_sasaran] > myTeam.listUnit.ElementAt(act.index_sasaran).getMaxHP())
                            myHPn[act.index_sasaran] = myTeam.listUnit.ElementAt(act.index_sasaran).getMaxHP();
                    }
                    if (act.aksi == Aksi.use_item && act.item == Item.life_potion)
                    {
                        myHPn[act.index_sasaran] = myTeam.listUnit.ElementAt(act.index_sasaran).getMaxHP() / 2;
                        lifn -= 1;
                    }
                    if (act.aksi == Aksi.menyerang)
                    {
                        enHPn[act.index_sasaran] -= CalculationDamage(myTeam.listUnit.ElementAt(act.index_pelaku), enTeam.listUnit.ElementAt(act.index_sasaran));
                    }
                }
                
                Unit unit;
                if (output.Count >= countMedic(myTeam))
                {
                    if (output.Count() - countMedic(myTeam) >= getFirstMedic(myTeam)) unit = myTeam.listUnit.ElementAt(output.Count);
                    else unit = myTeam.listUnit.ElementAt(output.Count() - countMedic(myTeam));

                }
                else unit = myTeam.listUnit.ElementAt(output.Count + getFirstMedic(myTeam));
                
                if (!(unit.isDead()))
                {
                    if (unit is Medic)
                    {
                        //heal
                        //Medic.isTidakBisaCuring();
                        if (!((Medic)unit).isTidakBisaCuring())
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                if (found) break;
                                if (myHPn[j] > 0 && (myTeam.listUnit.ElementAt(j).getMaxHP() - myHPn.ElementAt(j)) >= 500 && !myTeam.listUnit.ElementAt(j).isDead())
                                {
                                    temp = new List<ElemenAksi>(11);
                                    copyoutput(output, temp);
                                    cost.Add(i + 500);
                                    temp.Add(unit.Heal(myTeam.listUnit.ElementAt(j).index, myTeam));
                                    if (temp.Count() == 11)
                                        found = true;
                                    pohon.Add(temp);
                                }
                            }
                        }
                    }
                    if (!(unit is Rider) && potn > 0)
                    {
                        //potion
                        for (int j = 0; j < 11; j++)
                        {
                            if (found) break;
                            if (myHPn.ElementAt(j) > 0 && (myTeam.listUnit.ElementAt(j).getMaxHP() - myHPn.ElementAt(j)) >= 500 && !myTeam.listUnit.ElementAt(j).isDead())
                            {
                                temp = new List<ElemenAksi>(11);
                                copyoutput(output, temp);
                                cost.Add(i+500);
                                temp.Add(unit.useItem(myTeam.listUnit.ElementAt(j).index, myTeam, Item.potion));
                                potn--;
                                if (temp.Count() == 11)
                                    found = true;
                                pohon.Add(temp);
                            }
                        }
                    }
                    if (!(unit is Rider) && lifep > 0)
                    {
                        //lpotion
                        for (int j = 0; j < 11; j++)
                        {
                            if (found) break;
                            if (myHPn.ElementAt(j) <= 0 && myTeam.listUnit.ElementAt(j).isDead())
                            {
                                int k;
                                temp = new List<ElemenAksi>(11);
                                copyoutput(output, temp);
                                k = 1000;
                                if (myTeam.listUnit.ElementAt(j) is Archer) k -= 250;
                                else if (myTeam.listUnit.ElementAt(j) is Rider) k += 500;
                                cost.Add(i + k);
                                temp.Add(unit.useItem(myTeam.listUnit.ElementAt(j).index, myTeam, Item.life_potion));
                                lifn--;
                                if (temp.Count() == 11)
                                    found = true;
                                pohon.Add(temp);
                            }
                        }
                    }
                    if (!(unit is Medic))
                    {
                        //attack
                        for (int j = 0; j < 11; j++)
                        {
                            if (found) break;
                            if (enHPn.ElementAt(j) > 0)
                            {
                                int k;
                                temp = new List<ElemenAksi>(11);
                                copyoutput(output, temp);
                                k = CalculationDamage(unit,enTeam.listUnit.ElementAt(j));
                                cost.Add(i+k);
                                temp.Add(unit.Attack(enTeam.listUnit.ElementAt(j).index, enTeam));
                                if (temp.Count() == 11)
                                    found = true;
                                pohon.Add(temp);
                            }
                        }
                    }
                }
                if (!found)
                {
                    temp = new List<ElemenAksi>(11);
                    copyoutput(output, temp);
                    cost.Add(i);
                    temp.Add(unit.Defend());
                    if (temp.Count() == 11)
                        found = true;
                    pohon.Add(temp);
                }
            }
            foreach (ElemenAksi act in temp)
            {
                if (act.aksi == Aksi.use_item && act.item == Item.potion) potion--;
                if (act.aksi == Aksi.use_item && act.item == Item.life_potion) lifep--;
            }
            //semiLog(temp,myTeam,enTeam);
            return temp;
        }

        private void copyoutput(List<ElemenAksi> output, List<ElemenAksi> temp)
        {
            foreach (ElemenAksi act in output)
            {
                temp.Add(act);
            }
        }

        private int CalculationDamage(Unit satu, Unit dua)
        {
            int DamageTaken = 200;

            if (satu is Archer)
            {
                if (dua is Rider)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Swordsman) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }
            else if (satu is Swordsman)
            {
                if (dua is Archer)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Spearman) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }
            else if (satu is Spearman)
            {
                if (dua is Swordsman)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Rider) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }
            else if (satu is Rider)
            {
                if (dua is Spearman)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Archer) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }

            return DamageTaken;
        }

        private int getFirstMedic(Team myTeam)
        {
            foreach (var unit in myTeam.listUnit)
            {
                if (unit is Medic) return unit.index;
            }
            return 0;
        }

        private int countMedic(Team myTeam)
        {
            int c = 0;
            foreach (var unit in myTeam.listUnit)
            {
                if (unit is Medic) c++;
            }
            return c;
        }

        private void semiLog(List<ElemenAksi> LA, Team myTeam, Team enTeam)
        {
            String S = "";
            String temp = "";
            S = "action:\n";
            foreach (var act in LA)
            {
                if (act.aksi == Aksi.bertahan) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Bertahan";
                else if (act.aksi == Aksi.menyerang) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Menyerang " + getJob(enTeam, act.index_sasaran) + getJobIndex(enTeam, act.index_sasaran);
                else if (act.aksi == Aksi.heal) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Meng-Heal " + getJob(myTeam, act.index_sasaran) + getJobIndex(myTeam, act.index_sasaran);
                else if (act.aksi == Aksi.use_item && act.item == Item.potion) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Memakai Potion kepada  " + getJob(myTeam, act.index_sasaran) + getJobIndex(myTeam, act.index_sasaran);
                else if (act.aksi == Aksi.use_item && act.item == Item.life_potion) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Memakai Life-Potion ke " + getJob(myTeam, act.index_sasaran) + getJobIndex(myTeam, act.index_sasaran);
                S += (temp + "\n");
            }
            S += "\nmyTeam:\n";
            foreach (var unit in myTeam.listUnit)
            {
                S += (unit.getCurrentHP()+"\t"+getJob(myTeam, unit.index) + getJobIndex(myTeam, unit.index) + "\n");
            }
            S += "\nenTeam:\n";
            foreach (var unit in enTeam.listUnit)
            {
                S += (unit.getCurrentHP()+"\t"+getJob(enTeam, unit.index) + getJobIndex(enTeam, unit.index)+ "\n");
            }
            MessageBox.Show(S);
        }
        private String getJob(Team myTeam, int index)
        {
            Unit unit = myTeam.listUnit.ElementAt(index);
            if (unit is Archer) return "Archer";
            else if (unit is Swordsman) return "Swordsman";
            else if (unit is Spearman) return "Spearman";
            else if (unit is Rider) return "Rider";
            else if (unit is Medic) return "Medic";
            return "error";
        }
        private int getJobIndex(Team myTeam, int index)
        {
            String Job = getJob(myTeam, index);
            int count = 0;
            foreach (var unit in myTeam.listUnit)
            {
                if (Job == getJob(myTeam, unit.index))
                {
                    if (unit.index != index) count++;
                    else break;
                }

            }
            return count;
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
        private int potion = 10;
        private int lifep = 10;

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
            if (!myTeam.isPotionRunOut() && potion <= 0) potion = 1;
            if (!myTeam.isLifePotionRunOut() && lifep <= 0) lifep = 1;
            int i;
            Boolean found = false;
            List<ElemenAksi> output = new List<ElemenAksi>(11);
            List<ElemenAksi> temp = new List<ElemenAksi>(11);
            List<List<ElemenAksi>> pohon = new List<List<ElemenAksi>>();
            List<int> cost = new List<int>();
            int[] myHP = new int[11];
            int[] enHP = new int[11];
            int potn;
            int lifn;
            for (i = 0; i < 11; i++)
            {
                if (!(myTeam.listUnit.ElementAt(i).isDead()))
                {
                    myHP[i] = myTeam.listUnit.ElementAt(i).getCurrentHP();
                }
                else
                {
                    myHP[i] = 0;
                }
                if (!(enTeam.listUnit.ElementAt(i).isDead()))
                {
                    enHP[i] = enTeam.listUnit.ElementAt(i).getCurrentHP();
                }
                else
                {
                    enHP[i] = 0;
                }
            }
            int[] myHPn = new int[11];
            int[] enHPn = new int[11];

            cost.Add(myHP.Sum() - enHP.Sum());
            pohon.Add(output);
            while (pohon.Count() > 0 && !found)
            {
                i = cost.Max();
                output = pohon.ElementAt(cost.IndexOf(i));
                pohon.RemoveAt(cost.IndexOf(i));
                cost.RemoveAt(cost.IndexOf(i));
                myHP.CopyTo(myHPn, 0);
                enHP.CopyTo(enHPn, 0);
                potn = potion;
                lifn = lifep;

                foreach (ElemenAksi act in output)
                {
                    if ((act.aksi == Aksi.heal) || (act.aksi == Aksi.use_item && act.item == Item.potion))
                    {
                        myHPn[act.index_sasaran] += 500;
                        if(act.aksi == Aksi.use_item)potn -= 1;
                        if (myHPn[act.index_sasaran] > myTeam.listUnit.ElementAt(act.index_sasaran).getMaxHP())
                            myHPn[act.index_sasaran] = myTeam.listUnit.ElementAt(act.index_sasaran).getMaxHP();
                    }
                    if (act.aksi == Aksi.use_item && act.item == Item.life_potion)
                    {
                        myHPn[act.index_sasaran] = myTeam.listUnit.ElementAt(act.index_sasaran).getMaxHP() / 2;
                        lifn -= 1;
                    }
                    if (act.aksi == Aksi.menyerang)
                    {
                        enHPn[act.index_sasaran] -= CalculationDamage(myTeam.listUnit.ElementAt(act.index_pelaku), enTeam.listUnit.ElementAt(act.index_sasaran));
                    }
                }
                
                Unit unit;
                if (output.Count >= countMedic(myTeam))
                {
                    if (output.Count() - countMedic(myTeam) >= getFirstMedic(myTeam)) unit = myTeam.listUnit.ElementAt(output.Count);
                    else unit = myTeam.listUnit.ElementAt(output.Count() - countMedic(myTeam));

                }
                else unit = myTeam.listUnit.ElementAt(output.Count + getFirstMedic(myTeam));
                
                if (!(unit.isDead()))
                {
                    if (unit is Medic)
                    {
                        //heal
                        //Medic.isTidakBisaCuring();
                        if (!((Medic)unit).isTidakBisaCuring())
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                if (found) break;
                                if (myHPn[j] > 0 && (myTeam.listUnit.ElementAt(j).getMaxHP() - myHPn.ElementAt(j)) >= 500 && !myTeam.listUnit.ElementAt(j).isDead())
                                {
                                    temp = new List<ElemenAksi>(11);
                                    copyoutput(output, temp);
                                    //astar
                                    if ((myHPn[j] < 500) && !(myTeam.listUnit.ElementAt(j) is Rider))
                                        i += 900;
                                    cost.Add(i + 500);
                                    temp.Add(unit.Heal(myTeam.listUnit.ElementAt(j).index, myTeam));
                                    if (temp.Count() == 11)
                                        found = true;
                                    pohon.Add(temp);
                                }
                            }
                        }
                    }
                    if (!(unit is Rider) && potn > 0)
                    {
                        //potion
                        for (int j = 0; j < 11; j++)
                        {
                            if (found) break;
                            if (myHPn.ElementAt(j) > 0 && (myTeam.listUnit.ElementAt(j).getMaxHP() - myHPn.ElementAt(j)) >= 500 && !myTeam.listUnit.ElementAt(j).isDead())
                            {
                                temp = new List<ElemenAksi>(11);
                                copyoutput(output, temp);
                                //astar
                                if ((myHPn[j] < 500) && !(myTeam.listUnit.ElementAt(j) is Rider))
                                    i += 900;
                                cost.Add(i+500);
                                temp.Add(unit.useItem(myTeam.listUnit.ElementAt(j).index, myTeam, Item.potion));
                                potn--;
                                if (temp.Count() == 11)
                                    found = true;
                                pohon.Add(temp);
                            }
                        }
                    }
                    if (!(unit is Rider) && lifep > 0)
                    {
                        //lpotion
                        for (int j = 0; j < 11; j++)
                        {
                            if (found) break;
                            if (myHPn.ElementAt(j) <= 0 && myTeam.listUnit.ElementAt(j).isDead())
                            {
                                int k;
                                int max=0;
                                int[] enemy = new int[5];
                                for (k = 0; k < 5; k++) enemy[k] = 0;
                                for (k = 0; k < 11; k++)
                                {
                                    if ((enTeam.listUnit.ElementAt(k) is Medic) && (enTeam.listUnit.ElementAt(k).isDead())) enemy[0]++;
                                    if ((enTeam.listUnit.ElementAt(k) is Rider) && (enTeam.listUnit.ElementAt(k).isDead())) enemy[1]++;
                                    if ((enTeam.listUnit.ElementAt(k) is Swordsman) && (enTeam.listUnit.ElementAt(k).isDead())) enemy[2]++;
                                    if ((enTeam.listUnit.ElementAt(k) is Spearman) && (enTeam.listUnit.ElementAt(k).isDead())) enemy[3]++;
                                    if ((enTeam.listUnit.ElementAt(k) is Archer) && (enTeam.listUnit.ElementAt(k).isDead())) enemy[4]++;
                                }
                                for (k = 0; k < 5; k++)
                                {
                                    if (enemy[k] > max)
                                        max = enemy[k];
                                }
                                //Mencari jenis musuh yang paling banyak
                                for (k = 0; k < 5; k++)
                                {
                                    if (enemy[k] == max)
                                        break;
                                }
                                max = k;
                                temp = new List<ElemenAksi>(11);
                                copyoutput(output, temp);
                                k = 1000;
                                if (myTeam.listUnit.ElementAt(j) is Archer) k -= 250;
                                else if (myTeam.listUnit.ElementAt(j) is Rider) k += 500;
                                //Medic tidak dihidupkan lagi jika tidak bisa melakukan heal
                                if ((myTeam.listUnit.ElementAt(j) is Medic) && (((Medic)myTeam.listUnit.ElementAt(j)).isTidakBisaCuring()))
                                    k -= 500;
                                else if ((myTeam.listUnit.ElementAt(j) is Medic) && !(((Medic)myTeam.listUnit.ElementAt(j)).isTidakBisaCuring()))
                                    k += 500;
                                //Prioritas revive tergantung komposisi lawan
                                if ((max == 1) && (myTeam.listUnit.ElementAt(j) is Spearman)) k += 600;
                                else if ((max == 2) && (myTeam.listUnit.ElementAt(j) is Archer)) k += 600;
                                else if ((max == 3) && (myTeam.listUnit.ElementAt(j) is Swordsman)) k += 600;
                                else if ((max == 4) && (myTeam.listUnit.ElementAt(j) is Rider)) k += 600;
                                cost.Add(i + k);
                                temp.Add(unit.useItem(myTeam.listUnit.ElementAt(j).index, myTeam, Item.life_potion));
                                lifn--;
                                if (temp.Count() == 11)
                                    found = true;
                                pohon.Add(temp);
                            }
                        }
                    }
                    if (!(unit is Medic))
                    {
                        //attack
                        for (int j = 0; j < 11; j++)
                        {
                            if (found) break;
                            if (enHPn.ElementAt(j) > 0)
                            {
                                int k;
                                temp = new List<ElemenAksi>(11);
                                copyoutput(output, temp);
                                k = CalculationDamage(unit,enTeam.listUnit.ElementAt(j));
                                //astar
                                if (enHPn.ElementAt(j) < k)
                                    k += 1000;
                                cost.Add(i+k);
                                temp.Add(unit.Attack(enTeam.listUnit.ElementAt(j).index, enTeam));
                                if (temp.Count() == 11)
                                    found = true;
                                pohon.Add(temp);
                            }
                        }
                    }
                }
                if (!found)
                {
                    temp = new List<ElemenAksi>(11);
                    copyoutput(output, temp);
                    cost.Add(i);
                    temp.Add(unit.Defend());
                    if (temp.Count() == 11)
                        found = true;
                    pohon.Add(temp);
                }
            }
            foreach (ElemenAksi act in temp)
            {
                if (act.aksi == Aksi.use_item && act.item == Item.potion) potion--;
                if (act.aksi == Aksi.use_item && act.item == Item.life_potion) lifep--;
            }
            //semiLog(temp,myTeam,enTeam);
            return temp;
        }

        private void copyoutput(List<ElemenAksi> output, List<ElemenAksi> temp)
        {
            foreach (ElemenAksi act in output)
            {
                temp.Add(act);
            }
        }

        private int CalculationDamage(Unit satu, Unit dua)
        {
            int DamageTaken = 200;

            if (satu is Archer)
            {
                if (dua is Rider)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Swordsman) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }
            else if (satu is Swordsman)
            {
                if (dua is Archer)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Spearman) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }
            else if (satu is Spearman)
            {
                if (dua is Swordsman)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Rider) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }
            else if (satu is Rider)
            {
                if (dua is Spearman)//kelemahan
                    DamageTaken /= 2;
                else if ((dua is Archer) || (dua is Medic)) //kuat
                    DamageTaken = (int)(DamageTaken * 1.5);
            }

            return DamageTaken;
        }

        private int getFirstMedic(Team myTeam)
        {
            foreach (var unit in myTeam.listUnit)
            {
                if (unit is Medic) return unit.index;
            }
            return 0;
        }

        private int countMedic(Team myTeam)
        {
            int c = 0;
            foreach (var unit in myTeam.listUnit)
            {
                if (unit is Medic) c++;
            }
            return c;
        }

        private void semiLog(List<ElemenAksi> LA, Team myTeam, Team enTeam)
        {
            String S = "";
            String temp = "";
            S = "action:\n";
            foreach (var act in LA)
            {
                if (act.aksi == Aksi.bertahan) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Bertahan";
                else if (act.aksi == Aksi.menyerang) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Menyerang " + getJob(enTeam, act.index_sasaran) + getJobIndex(enTeam, act.index_sasaran);
                else if (act.aksi == Aksi.heal) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Meng-Heal " + getJob(myTeam, act.index_sasaran) + getJobIndex(myTeam, act.index_sasaran);
                else if (act.aksi == Aksi.use_item && act.item == Item.potion) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Memakai Potion kepada  " + getJob(myTeam, act.index_sasaran) + getJobIndex(myTeam, act.index_sasaran);
                else if (act.aksi == Aksi.use_item && act.item == Item.life_potion) temp = "" + getJob(myTeam, act.index_pelaku) + getJobIndex(myTeam, act.index_pelaku) + " Memakai Life-Potion ke " + getJob(myTeam, act.index_sasaran) + getJobIndex(myTeam, act.index_sasaran);
                S += (temp + "\n");
            }
            S += "\nmyTeam:\n";
            foreach (var unit in myTeam.listUnit)
            {
                S += (unit.getCurrentHP()+"\t"+getJob(myTeam, unit.index) + getJobIndex(myTeam, unit.index) + "\n");
            }
            S += "\nenTeam:\n";
            foreach (var unit in enTeam.listUnit)
            {
                S += (unit.getCurrentHP()+"\t"+getJob(enTeam, unit.index) + getJobIndex(enTeam, unit.index)+ "\n");
            }
            MessageBox.Show(S);
        }
        private String getJob(Team myTeam, int index)
        {
            Unit unit = myTeam.listUnit.ElementAt(index);
            if (unit is Archer) return "Archer";
            else if (unit is Swordsman) return "Swordsman";
            else if (unit is Spearman) return "Spearman";
            else if (unit is Rider) return "Rider";
            else if (unit is Medic) return "Medic";
            return "error";
        }
        private int getJobIndex(Team myTeam, int index)
        {
            String Job = getJob(myTeam, index);
            int count = 0;
            foreach (var unit in myTeam.listUnit)
            {
                if (Job == getJob(myTeam, unit.index))
                {
                    if (unit.index != index) count++;
                    else break;
                }

            }
            return count;
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
                            temp2.ElementAt(myUnit.index).Add(myUnit.Heal(myUnit.index, myTeam));
                        }
                        foreach (var myunit in myTeam.listUnit)
                        {
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myUnit.index, myTeam, Item.life_potion));
                        }
                        foreach (var myunit in myTeam.listUnit)
                        {
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myUnit.index, myTeam, Item.potion));
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
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myUnit.index, myTeam, Item.life_potion));
                        }
                        foreach (var myunit in myTeam.listUnit)
                        {
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myUnit.index, myTeam, Item.potion));
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
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myUnit.index, myTeam, Item.life_potion));
                        }
                        foreach (var myunit in myTeam.listUnit)
                        {
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myUnit.index, myTeam, Item.potion));
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
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myUnit.index, myTeam, Item.life_potion));
                        }
                        foreach (var myunit in myTeam.listUnit)
                        {
                            temp2.ElementAt(myUnit.index).Add(myUnit.useItem(myUnit.index, myTeam, Item.potion));
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
