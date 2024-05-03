namespace Zombieshooter
{
    public partial class Form1 : Form
    {
        // lista som innehåller alla levande zombies
        List<Zombie> zombieList = new List<Zombie>();

        // vapen med olika egenskaper
        Weapon revolver = new Weapon(50, TimeSpan.FromMilliseconds(200));
        Weapon shotgun = new Weapon(200, TimeSpan.FromMilliseconds(600));

        // antal poäng
        int score;

        // ljudeffekt för shotgun
        System.Media.SoundPlayer shotgunSound =
            new System.Media.SoundPlayer(Properties.Resources.shotgun_sound);

        // ljudeffekt för revolver
        System.Media.SoundPlayer revolverSound =
            new System.Media.SoundPlayer(Properties.Resources.revolver_sound);

        // ljudeffekt för när spelaren dör
        System.Media.SoundPlayer deathSound =
            new System.Media.SoundPlayer(Properties.Resources.wilhelm_scream);

        // ljudeffekt för när zombie dör
        System.Media.SoundPlayer zombieDeathSound =
            new System.Media.SoundPlayer(Properties.Resources.zombie_death);

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 1. Avfyra shotgunen om det inte överskrider "firing rate"
        /// 2. Den närmase zombien skadas
        /// 3. Om den skadade zombien förlorar alla sina HP dör den och spelaren får poäng
        /// </summary>
        private void picShotgun_Click(object sender, EventArgs e)
        {
            fireWeapon(shotgun, shotgunSound);
        }

        /// <summary>
        /// 1. Avfyra revolvern om det inte överskrider "firing rate"
        /// 2. Den närmase zombien skadas
        /// 3. Om den skadade zombien förlorar alla sina HP dör den och spelaren får poäng
        /// </summary>
        private void picRevolver_Click(object sender, EventArgs e)
        {
            fireWeapon(revolver, revolverSound);
        }

        private void fireWeapon(Weapon weapon, System.Media.SoundPlayer sound)
        {
            bool didFire = weapon.Fire();
            if (didFire)
            {
                // skada första zombien
                sound.Play();
                if (zombieList.Count > 0)
                {
                    Zombie zombie = zombieList[0];
                    zombie.Shoot(weapon);

                    // zombie dör om slut på hitpoints
                    if (zombie.NoHitpoints())
                    {
                        score++;
                        updateScoreLabel();
                        zombieDeathSound.Play();
                        zombieList.RemoveAt(0);
                    }
                }
            }
        }

        private void updateScoreLabel()
        {
            labelScore.Text = "Score: " + score;
            labelScore.BringToFront();
        }

        /// <summary>
        /// Kontrollera om en zombie kommit hela vägen fram. Kommer en zombie hela vägen fram
        /// förlorar man spelet.
        /// </summary>
        private void loseGameIfZombieIsBiting()
        {
            if (zombieList.Count > 0 && zombieList[0].IsBiting())
            {
                timerMove.Enabled = false;
                timerSpawn.Enabled = false;
                labelDied.Visible = true;
                labelDied.BringToFront();
            }
        }

        /// <summary>
        /// Flytta alla zombies. Kontrollera om någon kommit ända fram.
        /// </summary>
        private void timerMove_Tick(object sender, EventArgs e)
        {
            foreach (Zombie zombie in zombieList)
            {
                zombie.Move(timerMove.Interval);
            }
            loseGameIfZombieIsBiting();
        }

        /// <summary>
        /// Skapa (eng. spawn) en ny zombie och lägg till i listan.
        /// </summary>
        private void timerSpawn_Tick(object sender, EventArgs e)
        {
            newZombie();
        }

        private void newZombie()
        {
            Zombie zombie = new Zombie(this, 800, 15, 0);
            zombieList.Add(zombie);
        }

        /// <summary>
        /// Starta spelet när man klickar på knappen.
        /// </summary>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            timerMove.Start();
            timerSpawn.Start();
            score = 0;
            updateScoreLabel();
            zombieList.Clear();
            labelDied.Visible = false;
            newZombie();
        }
    }
}