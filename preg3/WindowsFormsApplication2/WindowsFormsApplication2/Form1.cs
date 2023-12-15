using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Linq;//not
using System.Text;
using System.Threading.Tasks;//not
using System.Windows.Forms;

using System.Drawing.Imaging;
using System.Data.SqlClient;


namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {

        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        string connectionString = "Data Source=(local);Initial Catalog=inf324;Integrated Security=True;";
        int Rm, Gm, Bm;
        
        int Rt, Gt, Bt;
        int L = 10;

        public Form1()
        {
            InitializeComponent();

            
        }
        private void cargar_img_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = string.Empty;

            openFileDialog1.Filter = "Archivos JPG|*.JPG|Archivos BMP|*.bmp";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != string.Empty)
            {
                Bitmap bmp = new Bitmap(openFileDialog1.FileName);
                pictureBox1.Image = bmp;
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            Color c = new Color();
            c = bmp.GetPixel(e.X, e.Y);
            
            textBox1.Text = c.R.ToString();
            textBox2.Text = c.G.ToString();
            textBox3.Text = c.B.ToString();

            
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = "INSERT INTO color VALUES (@R, @G, @B)";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@R", textBox1.Text);
            command.Parameters.AddWithValue("@G", textBox2.Text);
            command.Parameters.AddWithValue("@B", textBox3.Text);
            
            command.ExecuteNonQuery();
            Console.WriteLine("Dato insertado correctamente");
            connection.Close();
        }

        private void insertar_col_Click(object sender, EventArgs e)
        {

            /*
             * -------------------USAS ESTA CONEXION IGUAL FUNCIONA (En todas Partes)-------
             * -------------------PARA QUE NO SEA TODO IGUAL
             *
            SqlConnection con = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            con.ConnectionString ="Data Source= (local) ;Initial Catalog=color;Integrated Security=True;";
            con.Open();
            cmd.Connection = con;
             
             //consulta ejeplo
            cmd.CommandText = "insert into color values(123,555,888)";
             
            cmd.ExecuteNonQuery();
            con.Close();
             
            */
            
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = "INSERT INTO color VALUES (@R, @G, @B)";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@R", textBox1.Text);
            command.Parameters.AddWithValue("@G", textBox2.Text);
            command.Parameters.AddWithValue("@B", textBox3.Text);
            command.ExecuteNonQuery();
            connection.Close();
             
        }

        private void pintar_col_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp.Width, bmp.Height);
            Color c = new Color();

            List<Tuple<int, int, int>> listaColores = ObtenerElementosDesdeBaseDeDatos();
            foreach (var color in listaColores)
            {
                Console.WriteLine("R:" +color.Item1+"G:" +color.Item2 +" B:"+color.Item3);
                int rr = color.Item1;
                int gg = color.Item2;
                int bb = color.Item3;

                for (int i = 0; i < bmp.Width - L; i = i + L)
                {
                    for (int j = 0; j < bmp.Height - L; j = j + L)
                    {
                        Rt = 0; Gt = 0; Bt = 0;
                        for (int o = i; o < i + L; o++)
                            for (int p = j; p < j + L; p++)
                            {
                                c = bmp.GetPixel(o, p);
                                Rt += c.R;
                                Gt += c.G;
                                Bt += c.B;
                            }
                        Rt = Rt / (L * L); Gt = Gt / (L * L); Bt = Bt / (L * L);
                        if (((rr - L < Rt) && (Rt < rr + L))
                            && ((gg - L < Gt) && (Gt < gg + L))
                            && ((bb - L < Bt) && (Bt < bb + L)))
                        {
                            for (int o = i; o < i + L; o++)
                                for (int p = j; p < j + L; p++)
                                {
                                    bmp2.SetPixel(o, p, Color.FromArgb(Math.Abs(rr - 255), Math.Abs(gg - 255), Math.Abs(bb - 255)));
                                }
                        }
                        else
                        {
                            for (int o = i; o < i + L; o++)
                                for (int p = j; p < j + L; p++)
                                {
                                    c = bmp.GetPixel(o, p);
                                    bmp2.SetPixel(o, p, Color.FromArgb(c.R, c.G, c.B));
                                }
                        }
                    }
                }
                bmp = bmp2;

            }
            pictureBox1.Image = bmp2;
        }

        private List<Tuple<int, int, int>> ObtenerElementosDesdeBaseDeDatos()
        {
            List<Tuple<int, int, int>> colorList = new List<Tuple<int, int, int>>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT r, g, b FROM color";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int r = reader.GetInt32(0);
                            int g = reader.GetInt32(1);
                            int b = reader.GetInt32(2);

                            // Agregar la tupla a la lista
                            colorList.Add(Tuple.Create(r, g, b));
                        }
                    }
                }
                connection.Close();
            }
            return colorList;
        }

    }
}
