using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Threading.Tasks;

public class Fast_resize
{
    public Fast_resize():this(false)  {}
    public Fast_resize(bool source_order)
    {
        this.Source_inverse = source_order;
    }
    private int r_offset = 2, g_offfset = 1, b_offfset = 1;

    private bool color_inverse = false;
    public bool Source_inverse
    {
        get   {  return color_inverse;  }
        set
        {
            this.color_inverse = value;
            if (this.color_inverse == true)
            {
                r_offset = 0;
                g_offfset = 1;
                b_offfset = 2;
            }
            else
            {
                r_offset = 2;
                g_offfset = 1;
                b_offfset = 0;

            }
        }
    }

    public  unsafe Image<Rgb, float> resize_nearest(Mat originImage, Rectangle roi, int w, int h)
    {

  
            Image<Rgb, float> new_mat = new Image<Rgb, float>(w, h);
            byte* data = (byte*)originImage.DataPointer;
            int stride = originImage.Width;
            int orign_x = roi.X;
            int orign_y = roi.Y;
            float scale_x = roi.Width / (float)w;
            float scale_y = roi.Height / (float)h;
            int max_len = originImage.Width * originImage.Height * 3;

     
            Parallel.For(0, h, i =>
            {
                int x, y;
                int pos;
                
                y = (int)(i * scale_y) + orign_y;
                y = y * stride * 3;
                for (int j = 0; j < w; j++)
                {
                    x = (int)(j * scale_x) + orign_x;
                    pos = (int)(y + 3*x) ;
                    if (pos < 0 || pos >= max_len) continue;
                    new_mat.Data[i, j, 0] = data[pos + r_offset];
                    new_mat.Data[i, j, 1] = data[pos + g_offfset];
                    new_mat.Data[i, j, 2] = data[pos + b_offfset];
                }
            });

        

        return new_mat;
    }



    public unsafe Image<Rgb, float> resize_bilinear(Mat originImage, Rectangle roi, int w, int h)
    {

        Image<Rgb, float> new_mat = new Image<Rgb, float>(w, h);
        byte* data = (byte*)originImage.DataPointer;
        int stride = originImage.Width * 3;
        int orign_x = roi.X;
        int orign_y = roi.Y;

        float scale_x = roi.Width / (float)w;
        float scale_y = roi.Height / (float)h;
        int max_len = originImage.Width * originImage.Height * 3;


        Parallel.For(0, h, (i) =>
        {
            float x, y;
            float a, b;
            int pos, pos2, pos3, pos4;
            float sub_a, sub_b;

            y = (i * scale_y) + orign_y;
            b = y - (int)y;
            y = (int)y;
            y *= stride;
            sub_b = 1 - b;
            y += 3 * orign_x;

            for (int j = 0; j < w; j++)
            {

                x = (j * scale_x);
                a = x - (int)x;
                x = (int)x;
                pos = (int)(y + 3 * x);

                if (pos < 0 || pos >= max_len) continue;

                pos2 = pos + stride;
                pos3 = pos + 3;
                pos4 = pos + stride + 3;
                sub_a = 1 - a;

                new_mat.Data[i, j, 0] = (sub_a * sub_b * data[pos + r_offset] +
               sub_a * b * data[pos2 + r_offset] +
                   a * sub_b * data[pos3 + r_offset] +
                   a * b * data[pos4 + r_offset]);

                new_mat.Data[i, j, 1] = (sub_a * sub_b * data[pos + g_offfset] +
               sub_a * b * data[pos2 + g_offfset] +
                   a * sub_b * data[pos3 + g_offfset] +
                   a * b * data[pos4 + g_offfset]);

                new_mat.Data[i, j, 2] = (sub_a * sub_b * data[pos+b_offfset] +
                    sub_a * b * data[pos2+ b_offfset] +
                     a * sub_b * data[pos3+ b_offfset] +
                     a * b * data[pos4+ b_offfset]) ;                           
            }
        });



        return new_mat;
    }
}

