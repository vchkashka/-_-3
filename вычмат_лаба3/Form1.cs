using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace вычмат_лаба3
{
    public partial class Form1 : Form
    {
        List<float> x = new List<float>();
        List<float> fx = new List<float>();
        //Создаем элемент Chart
        Chart myChart = new Chart();
        


        public Form1()
        {
            InitializeComponent();
            //Добавляем chart на форму и растягиваем по ширине формы
            myChart.Parent = this;
            myChart.Dock = DockStyle.Fill;
            //Добавляем в Chart область для рисования графиков
            myChart.ChartAreas.Add(new ChartArea("Math functions"));
            myChart.Hide();
            button3.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }



        //Метод Гаусса для решения матриц с выбором главного элемента по столбцу
        public float[] Gauss(float[][] array)
        {
            int k = 0, index = 0;
            float max = 0;
            float[] a = new float[array.Count()];
            for (int j = 0; j < array[0].Count(); j++)
            {
                //Ищем максимальный по модулю коэффициент
                for (int i = 0; i < array.Count(); i++)
                {
                    if (Math.Abs(array[i][j]) > max)
                    {
                        max = array[i][j];
                        a = array[i];
                        index = i;
                    }
                }
                //Меняем уравнения местами
                array[index] = array[k];
                array[k] = a;
                max = 0;
                float[][] array2 = new float[array.Count()][];
                for (int i = 0; i < array.Count(); i++)
                {
                    array2[i] = new float[array[0].Count()];
                    for (int h = 0; h < array[0].Count(); h++)
                        array2[i][h] = array[i][h];
                }
                //Приводим систему к треугольной матрице
                for (int i = k + 1; i < array.Count(); i++)
                {
                    for (int g = k; g < array[0].Count(); g++)
                    {
                        array[i][g] += array2[k][g] * (-array2[i][k] / array2[k][k]);
                    }
                }
                k++;
                if (k == array[0].Count() - 2) break;
            }

            //Обратный ход
            float[] result = new float[array.Count()];
            float summ = 0;
            //Находим xi в каждом уравнении
            for (int i = array.Count() - 1; i >= 0; i--)
            {
                for (int j = array.Count() - 1; j >= 0; j--)
                    summ += array[i][j] * result[j];
                result[i] = (array[i][array[0].Count() - 1] - summ) / array[i][i];
                summ = 0;
            }
            return result;
        }

        //Поиск сглаживающего многочлена n-й степени
        public float Smoothing_Polynomial(float xi, List <float> x, List<float> fx, int step)
        {
            int n = x.Count();
            float[] c = new float[2 * step + 1]; float[] d = new float[step + 1];
            float[][] a = new float[step + 1][];
            float[] ai;
            for (int m = 0; m <= 2 * step; m++)
            {
                for (int k = 0; k < n; k++)
                {
                    //Поиск с
                    c[m] += (float)Math.Pow(x[k], m); 
                    if (m <= step)
                    {
                        //Поиск d
                        d[m] += fx[k] * (float)Math.Pow(x[k], m); 
                    }
                }
            }

            for (int i = 0; i <= step; i++)
            {
                a[i] = new float[step + 2];
                for (int j = 0; j <= step; j++)
                {
                    //Заполняем матрицу  a значениями с
                    a[i][j] = c[i + j]; 
                }
                //Свободные коээфициенты матрицы а - значения d
                a[i][step + 1] = d[i];
            }

            ai = Gauss(a); //находим коэффициенты многочлена

            float sum = ai[0];
            //Подставляем значения x и ищем значения многочлена
            for (int i = 1; i <= step; i++) 
            {
                float l = 1;
                for (int j = 1; j <= i; j++)
                {
                    l *= xi;
                }
                sum += ai[i] * l;
            }
            return sum;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.ColumnCount = Convert.ToInt32(textBox1.Text);
            dataGridView1.RowCount = 2;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            float start = (float)Convert.ToDouble(textBox2.Text);
            float end = (float)Convert.ToDouble(textBox3.Text);

            for (int j = 0; j < dataGridView1.ColumnCount; j++)
            {
                x.Add((float)Convert.ToDouble(dataGridView1[j, 0].Value));
                fx.Add((float)Convert.ToDouble(dataGridView1[j, 1].Value));
            }
           

            float sum = 0;
            foreach (int indexChecked in checkedListBox1.CheckedIndices)
            {
                //Построение многочлена с помощью Лагранжа
                if (indexChecked == 0)
                {
                    //Создаем набор точек
                    Series mySeriesOfPoint1 = new Series();
                    mySeriesOfPoint1.ChartType = SeriesChartType.Line;
                    mySeriesOfPoint1.ChartArea = "Math functions";
                    mySeriesOfPoint1.BorderWidth = 3;
                    //Подставляем значения x в многочлен - вычисляем точки
                    for (float xi = start; xi <= end; xi += 0.01F)
                    {                        
                        sum = 0;
                        for (int i = 0; i < x.Count(); i++)
                        {
                            float l = 1;
                            for (int j = 0; j < x.Count(); j++)
                            {
                                if (j != i)
                                    l *= (xi - x[j]) / (x[i] - x[j]);
                            }
                            sum += l * fx[i];
                        }
                        mySeriesOfPoint1.Points.AddXY(xi, sum);
                    }
                    //Добавляем созданный набор точек в Chart
                    myChart.Series.Add(mySeriesOfPoint1);
                }

                //Построение многочлена с помощью Ньютона
                if (indexChecked == 1)
                {
                    Series mySeriesOfPoint2 = new Series();
                    mySeriesOfPoint2.ChartType = SeriesChartType.Line;
                    mySeriesOfPoint2.ChartArea = "Math functions";
                    mySeriesOfPoint2.BorderWidth = 4;
                    for (float xi = start; xi <= end; xi += 0.01F)
                    {
                        sum = fx[0];
                        for (int i = 1; i < x.Count(); ++i) //подставляем значения х в многочлен
                        {
                            float F = 0;
                            for (int j = 0; j <= i; ++j)
                            {

                                float den = 1;
                                for (int k = 0; k <= i; ++k)
                                    if (k != j)
                                        den *= (x[j] - x[k]);
                                F += fx[j] / den;
                            }

                            for (int k = 0; k < i; ++k)
                                F *= (xi - x[k]);
                            sum += F;
                        }
                        mySeriesOfPoint2.Points.AddXY(xi, sum);
                    }
                    myChart.Series.Add(mySeriesOfPoint2);
                }

                //Сглаживающий многочлен 1 степени
                if (indexChecked == 2)
                {
                    Series mySeriesOfPoint3 = new Series();
                    mySeriesOfPoint3.ChartType = SeriesChartType.Line;
                    mySeriesOfPoint3.ChartArea = "Math functions";
                    mySeriesOfPoint3.BorderWidth = 5;
                    for (float xi = start; xi <= end; xi += 0.01F)
                    {
                        mySeriesOfPoint3.Points.AddXY(xi, Smoothing_Polynomial(xi, x, fx, 1));
                    }
                    myChart.Series.Add(mySeriesOfPoint3);
                }

                //Сглаживающий многочлен 2 степени
                if (indexChecked == 3)
                {
                    Series mySeriesOfPoint4 = new Series();
                    mySeriesOfPoint4.ChartType = SeriesChartType.Line;
                    mySeriesOfPoint4.ChartArea = "Math functions";
                    mySeriesOfPoint4.BorderWidth = 6;
                    for (float xi = start; xi <= end; xi += 0.01F)
                    {
                        mySeriesOfPoint4.Points.AddXY(xi, Smoothing_Polynomial(xi, x, fx, 2));
                    }
                    myChart.Series.Add(mySeriesOfPoint4);
                }

                //Сглаживающий многочлен 3 степени
                if (indexChecked == 4)
                {
                    Series mySeriesOfPoint5 = new Series();
                    mySeriesOfPoint5.ChartType = SeriesChartType.Line;
                    mySeriesOfPoint5.ChartArea = "Math functions";
                    mySeriesOfPoint5.BorderWidth = 7;
                    for (float xi = start; xi <= end; xi += 0.01F)
                    {
                        mySeriesOfPoint5.Points.AddXY(xi, Smoothing_Polynomial(xi, x, fx, 3));
                    }
                    myChart.Series.Add(mySeriesOfPoint5);
                }

                //Сглаживающий многочлен 1 степени, расчитанный вручную
                if (indexChecked == 5)
                {
                    Series mySeriesOfPoint6 = new Series();
                    mySeriesOfPoint6.ChartType = SeriesChartType.Line;
                    mySeriesOfPoint6.ChartArea = "Math functions";
                    mySeriesOfPoint6.BorderWidth = 7;
                    for (float xi = start; xi <= end; xi += 0.01F)
                    {
                        float yi = -3.19f * xi + 11.466f;
                        mySeriesOfPoint6.Points.AddXY(xi, yi);
                    }
                    myChart.Series.Add(mySeriesOfPoint6);
                }

                //Сглаживающий многочлен 2 степени, расчитанный вручную
                if (indexChecked == 6)
                {
                    Series mySeriesOfPoint7 = new Series();
                    mySeriesOfPoint7.ChartType = SeriesChartType.Line;
                    mySeriesOfPoint7.ChartArea = "Math functions";
                    mySeriesOfPoint7.BorderWidth = 7;
                    for (float xi = start; xi <= end; xi += 0.01F)
                    {
                        float yi = 0.045f * (float)Math.Pow(xi, 2) - 3.276f * xi + 11.291f;
                        mySeriesOfPoint7.Points.AddXY(xi, yi);
                    }
                    myChart.Series.Add(mySeriesOfPoint7);
                }

                //Сглаживающий многочлен 3 степени, расчитанный вручную
                if (indexChecked == 7)
                {
                    Series mySeriesOfPoint8 = new Series();
                    mySeriesOfPoint8.ChartType = SeriesChartType.Line;
                    mySeriesOfPoint8.ChartArea = "Math functions";
                    mySeriesOfPoint8.BorderWidth = 7;
                    for (float xi = start; xi <= end; xi += 0.01F)
                    {
                        float yi = 0.059f * (float)Math.Pow(xi, 3) - 0.138f * (float)Math.Pow(xi, 2) - 3.539f * xi + 11.789f;
                        mySeriesOfPoint8.Points.AddXY(xi, yi);
                    }
                    myChart.Series.Add(mySeriesOfPoint8);
                }

                //Произвольный многочлен 4й степени
                if (indexChecked == 8)
                {
                    Series mySeriesOfPoint9 = new Series();
                    mySeriesOfPoint9.ChartType = SeriesChartType.Line;
                    mySeriesOfPoint9.ChartArea = "Math functions";
                    mySeriesOfPoint9.BorderWidth = 7;
                    for (float xi = start; xi <= end; xi += 0.01F)
                    {
                        float yi = 0.329f * (float)Math.Pow(xi, 4) - 1.379f * (float)Math.Pow(xi, 3) - 1.192f * (float)Math.Pow(xi, 2) + 2.767f * xi + 12;
                        mySeriesOfPoint9.Points.AddXY(xi, yi);
                    }
                    myChart.Series.Add(mySeriesOfPoint9);
                }
            }

            Series serie = new Series();
            //Добавляем точки из таблицы
            for (int i = 0; i < x.Count(); i++)
            {
                serie.ChartType = SeriesChartType.Point;
                // DataPoint dp = new DataPoint(x[i], fx[i]);
                serie.MarkerStyle = MarkerStyle.Circle;
                serie.MarkerSize = 6;
                serie.MarkerColor = Color.Black;
                serie.IsValueShownAsLabel = true; //Показать значение в точке
                serie.Points.AddXY(x[i], fx[i]);               
            }
            myChart.Series.Add(serie);
            this.Height = this.Width;
            myChart.Show();
            button3.Show();
            myChart.BringToFront();
            button3.BringToFront();
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.myChart.Series.Clear();
            foreach (var series in myChart.Series)
            {
                series.Points.Clear();                
            }
            myChart.Hide();
            button3.Hide();
            this.Height = 361;
        }
    }
}
