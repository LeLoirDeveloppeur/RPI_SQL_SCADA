using MySqlConnector;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RPI_SQL_SCADA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgvRegisters.Columns.Add("Col_1", "BCM_MAPPED_INPUT");
            dgvRegisters.Columns.Add("Col_2", "REMAPPED_INPUT");
            dgvRegisters.Columns.Add("Col_3", "CURRENT_VALUE");
            readingInputsFromRaspberry();
        }
        private void readingInputsFromRaspberry()
        {
            try
            {
                // Change in sever= the IP Address for the one on your RPI Board (write on the cli hostname -I to know its IP)
                string myConnectionString = @"server=194.172.5.141; userid=REMOTE_DORMOUSE; password=raspberry; database=RPIDB;";
                MySqlConnection MSQ = new MySqlConnection(myConnectionString);
                string SQL_QUERY = "SELECT * FROM INPUT_READING_EVENTS;";
                MySqlCommand MS_COMMAND = new MySqlCommand(SQL_QUERY, MSQ);
                MSQ.Open();
                MySqlDataReader Reader = MS_COMMAND.ExecuteReader();
                dgvRegisters.Rows.Clear();
                while (Reader.Read())
                {
                    dgvRegisters.Rows.Add(Reader[0].ToString(), Reader[1].ToString(), Reader[2].ToString());
                }
                Reader.Close();
                MSQ.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
        private void updateValuesOnDisplay()
        {
            // Giving internal names to the variables from the DB
            int ix_LocalStatus = int.Parse(dgvRegisters[2, 0].Value.ToString());
            int ix_52Status = int.Parse(dgvRegisters[2, 1].Value.ToString());
            int ix_89BB1Status = int.Parse(dgvRegisters[2, 2].Value.ToString());
            int ix_89BB2Status = int.Parse(dgvRegisters[2, 3].Value.ToString());
            int ix_89LStatus = int.Parse(dgvRegisters[2, 4].Value.ToString());
            int ix_SF6Alarm = int.Parse(dgvRegisters[2, 5].Value.ToString());
            int ix_SF6Trip = int.Parse(dgvRegisters[2, 6].Value.ToString());
            int ix_OverCurrent = int.Parse(dgvRegisters[2, 7].Value.ToString());

            // Updating LEDs
            pbLedLocal.Image = imgLeds.Images[ix_LocalStatus];
            pbLed52.Image = imgLeds.Images[ix_52Status];
            pbLed89BB1.Image = imgLeds.Images[ix_89BB1Status];
            pbLed89BB2.Image = imgLeds.Images[ix_89BB2Status];
            pbLed89L.Image = imgLeds.Images[ix_89LStatus];
            pbLedSF6Alarm.Image = imgLeds.Images[ix_SF6Alarm];
            pbLedSF6Trip.Image = imgLeds.Images[ix_SF6Trip];
            pbLedOverCurrent.Image = imgLeds.Images[ix_OverCurrent];

            // Updating text
            if (ix_52Status == 1) lbl52.ForeColor = Color.Red; else lbl52.ForeColor = Color.Black;
            if (ix_89BB1Status == 1) lbl89BB1.ForeColor = Color.Red; else lbl89BB1.ForeColor = Color.Black;
            if (ix_89BB2Status == 1) lbl89BB2.ForeColor = Color.Red; else lbl89BB2.ForeColor = Color.Black;
            if (ix_89LStatus == 1) lbl89LINE.ForeColor = Color.Red; else lbl89LINE.ForeColor = Color.Black;
            
            // Updating symbols
            pb52.Image = imgSwitchgear.Images[ix_52Status];
            pb89BB1.Image = imgDisconnector.Images[ix_89BB1Status];
            pb89BB2.Image = imgDisconnector.Images[ix_89BB2Status];
            pb89LINE.Image = imgDisconnector.Images[ix_89LStatus];

            //Updating sections
            if(ix_89BB1Status==1 ||ix_89BB2Status==1)
            {
                pbSection1_1.BackColor = Color.Red;
                pbSection1_2.BackColor = Color.Red;
                pbSection1_3.BackColor = Color.Red;
                pbSection1_4.BackColor = Color.Red;
                if (ix_52Status == 1)
                {
                    pbSection2.BackColor = Color.Red;
                    if(ix_89LStatus == 1)
                    {
                        pbSection3_1.BackColor = Color.Red;
                        pbArrow.Image = imgArrow.Images[1];
                        lblDestination.ForeColor = Color.Yellow;
                    }
                    else
                    {
                        pbSection3_1.BackColor = Color.Black;
                        pbArrow.Image = imgArrow.Images[0];
                        lblDestination.ForeColor = Color.Black;

                    }
                }
                else
                {
                    pbSection2.BackColor = Color.Black;
                    pbSection3_1.BackColor = Color.Black;
                    pbArrow.Image = imgArrow.Images[0];
                    lblDestination.ForeColor = Color.Black;
                }

            }
            else
            {
                pbSection1_1.BackColor = Color.Black;
                pbSection1_2.BackColor = Color.Black;
                pbSection1_3.BackColor = Color.Black;
                pbSection1_4.BackColor = Color.Black;
                pbSection2.BackColor = Color.Black;
                pbSection3_1.BackColor = Color.Black;
                pbArrow.Image = imgArrow.Images[0];
                lblDestination.ForeColor = Color.Black;
            }
        }

        private void readerTimer_Tick(object sender, EventArgs e)
        {
            readingInputsFromRaspberry();
            // We'll update the screen if we've received the information from the RPI board
            if (dgvRegisters.Rows.Count > 1)
            {
                updateValuesOnDisplay();
            }

        }
    }
}
