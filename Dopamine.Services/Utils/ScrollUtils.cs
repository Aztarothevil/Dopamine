using Digimezzo.Foundation.Core.Utils;
using Dopamine.Services.Entities;
using Dopamine.Services.Lyrics;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Dopamine.Services.Utils
{
    public sealed class ScrollUtils
    {
        static int currentIndex = 0;
        static int contador = 0;
        static double position = 0;
        static double lastPosition = 0;

        public static bool IsFullyVisible(FrameworkElement child, FrameworkElement scrollViewer)
        {
            GeneralTransform childTransform = child.TransformToAncestor(scrollViewer);
            Rect childRectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), child.RenderSize));
            Rect ownerRectangle = new Rect(new Point(0, 0), scrollViewer.RenderSize);

            return ownerRectangle.Contains(childRectangle.TopLeft) & ownerRectangle.Contains(childRectangle.BottomRight);
        }

        public static void ScrollToListBoxItem(ListBox box, object itemObject, bool scrollOnlyIfNotInView)
        {
            LyricsLineViewModel item = (LyricsLineViewModel)itemObject;
            ScrollViewer scrollViewer = (ScrollViewer)VisualTreeUtils.GetDescendantByType(box, typeof(ScrollViewer));
            // Verify that the item is not visible. Only scroll if it is not visible.
            // ----------------------------------------------------------------------
            if (scrollOnlyIfNotInView && (item.Index == 0 || item.Index < 10))
            {
                FrameworkElement listBoxItem = (FrameworkElement)box.ItemContainerGenerator.ContainerFromItem(itemObject);

                if (scrollViewer != null && listBoxItem != null)
                {
                    //scrollViewer.ScrollToVerticalOffset(0);

                    if (scrollViewer.VerticalOffset != 0)
                    {
                        box.UpdateLayout();
                        box.ScrollIntoView(box.Items[0]);
                        box.UpdateLayout();
                        box.ScrollIntoView(itemObject);
                    }

                    //if (IsFullyVisible(listBoxItem, scrollViewer))
                    //{
                    //    return;
                    //}
                }
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
                currentIndex = item.Index;
                contador = 0;
                position = ((item.Index - 10) * 28);
                lastPosition = position + 28;
            }

            contador++;

            int modulo = (int)((item.Duration * 75) / 9200);

            if (contador % modulo == 0)
            {
                if (position <= lastPosition)
                {
                    position += 1;
                    scrollViewer.ScrollToVerticalOffset(position);
                }
            }

            //System.Console.WriteLine(lastPosition + "  -  " + item.Duration + "  -  " + position + "  -  " + modulo);


            //box.ScrollIntoView(box.Items[item.Index+25]);
            //box.UpdateLayout(); // This seems required to get correct positioning.

            // Scroll to the desired Item
            // --------------------------
            //box.UpdateLayout(); // This seems required to get correct positioning.
            //box.ScrollIntoView(itemObject);
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
                ScrollToListBoxItem(box, itemObject, true);
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
            catch (Exception)
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
                ScrollToListBoxItem(box, itemObject, true);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
