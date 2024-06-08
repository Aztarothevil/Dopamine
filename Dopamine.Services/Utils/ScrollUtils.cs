using Digimezzo.Foundation.Core.Utils;
using Dopamine.Services.Entities;
using Dopamine.Services.Lyrics;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Dopamine.Services.Utils
{
    public sealed class ScrollUtils
    {
        static int currentIndex = 0;
        static int contador = 0;
        static double position = 0;
        static double lastPosition = 0;
        static double lastPositionLyric = 0;
        static double lastDuration = 0;
        static double lastDurationLyrc = 0;

        public static bool IsFullyVisible(FrameworkElement child, FrameworkElement scrollViewer)
        {
            GeneralTransform childTransform = child.TransformToAncestor(scrollViewer);
            Rect childRectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), child.RenderSize));
            Rect ownerRectangle = new Rect(new Point(0, 0), scrollViewer.RenderSize);

            return ownerRectangle.Contains(childRectangle.TopLeft) & ownerRectangle.Contains(childRectangle.BottomRight);
        }

        public static void ScrollToTextListItem(ListBox box, object itemObject, bool scrollOnlyIfNotInView)
        {
            LyricsLineViewModel item = (LyricsLineViewModel)itemObject;
            ScrollViewer scrollViewer = (ScrollViewer)VisualTreeUtils.GetDescendantByType(box, typeof(ScrollViewer));
            // Verify that the item is not visible. Only scroll if it is not visible.
            // ----------------------------------------------------------------------
            if (scrollOnlyIfNotInView && (item.Index == 0 || item.Index < 10))
            {
                //System.Console.WriteLine("lastPosition: " + lastPosition + "  -  Duration: " + item.Duration + "  -  Position: " + position + "  -  Contador: " + contador);
                FrameworkElement listBoxItem = (FrameworkElement)box.ItemContainerGenerator.ContainerFromItem(itemObject);

                if (scrollViewer != null && listBoxItem != null)
                {

                    if (scrollViewer.VerticalOffset != 0)
                    {
                        box.UpdateLayout();
                        box.ScrollIntoView(box.Items[0]);
                        box.UpdateLayout();
                        box.ScrollIntoView(itemObject);
                    }
                }
                lastPosition = 0;
                position = 0;
                contador = 0;
                return;
            }

            if (item.Duration == 0)
            {
                return;
            }

            // Scroll to the bottom of the list. This is a workaround which puts the 
            // desired Item at the top of the list when executing ScrollIntoView
            // ---------------------------------------------------------------------

            if (currentIndex != item.Index)
            {
                lastPositionLyric = ((item.Index - 10) * 28) + 28;
                lastDurationLyrc = item.Duration;

                if ((currentIndex < item.Index - 1 || currentIndex > item.Index + 1) && item.Index > 11)
                {
                    lastPosition = lastPositionLyric;
                    position = lastPositionLyric;
                    scrollViewer.ScrollToVerticalOffset(position);
                }

                currentIndex = item.Index;
                contador = 0;
            }

            if (lastPosition == position)
            {
                lastPosition = lastPositionLyric;
                lastDuration = lastDurationLyrc;
                contador = 0;
            }


            contador++;

            int modulo = (int)((lastDuration * 40) / 9200);

            if (contador % modulo == 0)
            {
                if (position <= lastPosition)
                {
                    position += 1;
                    scrollViewer.ScrollToVerticalOffset(position);
                }
            }

            //System.Console.WriteLine("lastPosition: " + lastPosition + "  -  Duration: " + item.Duration + "  -  Position: " + position + "  -  Modulo: " + modulo + "  -  Contador: " + contador);
        }

        public static void ScrollToListBoxItem(ListBox box, object itemObject, bool scrollOnlyIfNotInView)
        {
            // Verify that the item is not visible. Only scroll if it is not visible.
            // ----------------------------------------------------------------------
            if (scrollOnlyIfNotInView)
            {
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeUtils.GetDescendantByType(box, typeof(ScrollViewer));
                FrameworkElement listBoxItem = (FrameworkElement)box.ItemContainerGenerator.ContainerFromItem(itemObject);

                int index = ((Dopamine.Services.Entities.SongViewModel)itemObject).Index;
                int columns = (int)(scrollViewer.ActualWidth / 132);
                int rows = (int)(scrollViewer.ActualHeight / 168);
                int currentRow = ((index - 1) / columns) + 1;
                int position = (currentRow-1) * 178;
                int diference = 0;
                if ((scrollViewer.VerticalOffset - position) >= 10)
                {
                    scrollViewer.ScrollToVerticalOffset(position);
                }
                else if ((scrollViewer.VerticalOffset - position) <= (rows * -178))
                {
                    diference = (int)(scrollViewer.ActualHeight) - ((rows-1) * 178);
                    int remove = 0;
                    if (diference < 180)
                    {
                        remove = diference + ((180 - diference) * 2);
                    }
                    else
                    {
                        remove = diference - ((diference - 180) * 2);
                    }

                    scrollViewer.ScrollToVerticalOffset(position - ((rows * 178) - remove));
                }

                System.Console.WriteLine("Fila: " + currentRow + "  - Columnas: " + columns + "  - Filas " + rows + "  -  Indice: " + index + "  -  Current Position: " + position + " - VerticalOffset: " + scrollViewer.VerticalOffset + " - Diference: " + diference);
                
            }

        }

        public static void ScrollToDataGridItem(DataGrid grid, object itemObject, bool scrollOnlyIfNotInView)
        {
            // Verify that the item is not visible. Only scroll if it is not visible.
            // ----------------------------------------------------------------------
            if (scrollOnlyIfNotInView)
            {
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeUtils.GetDescendantByType(grid, typeof(ScrollViewer));
                FrameworkElement listBoxItem = (FrameworkElement)grid.ItemContainerGenerator.ContainerFromItem(itemObject);

                if (scrollViewer != null && listBoxItem != null)
                {
                    if (IsFullyVisible(listBoxItem, scrollViewer))
                    {
                        return;
                    }
                }
            }

            // Scroll to the bottom of the list. This is a workaround which puts the 
            // desired Item at the top of the list when executing ScrollIntoView
            // ---------------------------------------------------------------------
            grid.UpdateLayout(); // This seems required to get correct positioning.
            grid.ScrollIntoView(grid.Items[grid.Items.Count - 1]);

            // Scroll to the desired Item
            // --------------------------
            grid.UpdateLayout(); // This seems required to get correct positioning.
            grid.ScrollIntoView(itemObject);
        }

        public static async Task ScrollToPlayingTrackAsync(ListBox box)
        {
            if (box == null) return;

            Object itemObject = null;

            await Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i <= box.Items.Count - 1; i++)
                    {
                        if (((TrackViewModel)box.Items[i]).IsPlaying)
                        {
                            itemObject = box.Items[i];
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });

            if (itemObject == null)
            {
                return;
            }

            try
            {
                ScrollToTextListItem(box, itemObject, true);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static async Task ScrollToPlayingSongAsync(ListBox box)
        {
            if (box == null) return;

            Object itemObject = null;

            await Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i <= box.Items.Count - 1; i++)
                    {
                        if (((SongViewModel)box.Items[i]).IsPlaying)
                        {
                            itemObject = box.Items[i];
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });

            if (itemObject == null)
            {
                return;
            }

            try
            {
                ScrollToListBoxItem(box, itemObject, true);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task ScrollToPlayingTrackAsync(DataGrid grid)
        {
            if (grid == null) return;

            Object itemObject = null;

            await Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i <= grid.Items.Count - 1; i++)
                    {
                        if (((TrackViewModel)grid.Items[i]).IsPlaying)
                        {
                            itemObject = grid.Items[i];
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });

            if (itemObject == null) return;

            try
            {
                ScrollToDataGridItem(grid, itemObject, true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task ScrollToHighlightedLyricsLineAsync(ListBox box)
        {
            if (box == null)
            {
                return;
            }

            object itemObject = null;

            await Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i <= box.Items.Count - 1; i++)
                    {
                        if (box.Items[i] is LyricsLineViewModel && ((LyricsLineViewModel)box.Items[i]).IsHighlighted)
                        {
                            itemObject = box.Items[i];
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });

            if (itemObject == null) return;

            try
            {
                ScrollToTextListItem(box, itemObject, true);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
