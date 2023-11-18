using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Threading.Tasks;

public class Fast_resize
{
    public enum Color_type
    {
        RGB,BGR
    }
    private Color_type source_color_type = Color_type.BGR;
    private int r_offset=2,g_offfset=1,b_offfset=1;
    public Color_type Source_color_type
    {
        get { return source_color_type;}

        set
        {
            if(value == Color_type.RGB)
            {
                r_offset = 0; g_offfset=1; b_offfset=2;
            }
            else
            {
                r_offset = 2; g_offfset = 1; b_offfset = 0;
            }
            source_color_type = value;
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
            for (int j = 0; j < w; j++)
            {

                x = (int)(j * scale_x) + orign_x;

                pos = (int)(y * stride + x) * 3;

                if (pos < 0 || pos >= max_len) continue;

                new_mat.Data[i, j, 0] = data[pos + r_offset];
                new_mat.Data[i, j, 1] = data[pos + g_offfset];
                new_mat.Data[i, j, 2] = data[pos + b_offfset];
            }
        });

        return new_mat;
    }

}

