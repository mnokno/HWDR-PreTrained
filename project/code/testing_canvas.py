from tkinter import *
from tkinter import ttk

import numpy
from PIL import ImageGrab
from PIL import Image
from helper import Classifier


class Main:
    def __init__(self, master):
        self.classifier = Classifier()
        self.master = master

        self.color_fg = 'black'
        self.color_bg = 'white'
        self.old_x = None
        self.old_y = None
        self.pen_width = 5

        self.slider = None
        self.c = None
        self.simple_prediction_frame = None
        self.production_label = None
        self.confidence_label = None
        self.advancedPredictionFrame = None
        self.advanced_prediction_widgets = None
        self.draw_widgets()

        # c will be assigned in self.draw_widgets() hence it will be a Canvas not None
        # noinspection PyUnresolvedReferences
        self.c.bind('<B1-Motion>', self.paint)  # drawing on the canvas
        # c will be assigned in self.draw_widgets() hence it will be a Canvas not None
        # noinspection PyUnresolvedReferences
        self.c.bind('<ButtonRelease-1>', self.reset)
        self.detailed_prediction_report = False

    # Extracts the image from the canvas ready to be fed into classifier
    def get_img(self) -> Image:
        x = root.winfo_rootx() + self.c.winfo_x()
        y = root.winfo_rooty() + self.c.winfo_y()
        x1 = x + self.c.winfo_width()
        y1 = y + self.c.winfo_height()
        img = ImageGrab.grab().crop((x, y, x1, y1))
        return Classifier.format_image(img, invert_colors=True)

    # Saves extracted image to file and open preview
    # NOTE: the preview opened on window will automatically blur the image when zoomed in
    def preview_input_img(self) -> None:
        img = self.get_img()
        img.show()
        img.save("../img_preview/digit.jpg")

    # Handles painting on the canvas
    def paint(self, e) -> None:
        if self.old_x and self.old_y:

            self.c.create_line(self.old_x, self.old_y, e.x, e.y, width=self.pen_width, fill="black", capstyle=ROUND, smooth=True, splinesteps=12)

        self.old_x = e.x
        self.old_y = e.y
        img_class, data = self.classifier.classify(self.get_img())
        self.production_label.config(text=img_class)
        if numpy.max(data) > 0.1:
            self.confidence_label.config(text="{:0.1f}%".format(numpy.max(data) * 100))
        else:
            self.confidence_label.config(text="{:0.1f}0%".format(numpy.max(data) * 100))
        self.advanced_prediction_widgets["production"].config(text=img_class)
        for i in range(10):
            if data[i] > 0.1:
                self.advanced_prediction_widgets[str(i)].config(
                    text=str(i) + "({:0.1f}%)".format(data[i] * 100))
            else:
                self.advanced_prediction_widgets[str(i)].config(
                    text=str(i) + "({:0.1f}0%)".format(data[i] * 100))

    # Resting or cleaning the canvas
    def reset(self, e) -> None:
        self.old_x = None
        self.old_y = None

    # Change Width of pen through slider
    def changeW(self, e) -> None:
        self.pen_width = e

    # Clears the canvas
    def clear(self) -> None:
        self.c.delete(ALL)

    # Changes view between detailed and simple
    def change_view(self) -> None:
        self.detailed_prediction_report = not self.detailed_prediction_report

        if self.detailed_prediction_report:
            self.simple_prediction_frame.grid_remove()
            self.advancedPredictionFrame.grid(row=0, column=1)
        else:
            self.simple_prediction_frame.grid(row=0, column=1)
            self.advancedPredictionFrame.grid_remove()

    # Displays all widgets
    def draw_widgets(self) -> None:

        # Frame for Label and Slider
        sliderFrame = Frame(self.master)
        sliderFrame.grid(row=1, column=0)
        # Slider Label
        Label(sliderFrame, text='Pen Width:', font='arial 18').grid(row=0, column=0)
        # Slider
        self.slider = ttk.Scale(sliderFrame, from_=30, to=60, command=self.changeW, orient=HORIZONTAL)
        self.slider.set(self.pen_width)
        self.slider.grid(row=0, column=1, ipadx=168)

        # Canvas
        self.c = Canvas(self.master, width=560, height=560, bg=self.color_bg)
        self.c.grid(row=0, column=0, padx=5)

        # Frame for simple prediction
        self.simple_prediction_frame = Frame(self.master)
        self.simple_prediction_frame.grid(row=0, column=1)
        # Production
        self.production_label = Label(self.simple_prediction_frame, text="0", font='arial 300')
        self.production_label.grid(row=0, column=0, sticky="nw")
        self.confidence_label = Label(self.simple_prediction_frame, text="0%", font='arial 50')
        self.confidence_label.grid(row=1, column=0, sticky="n")

        # Frame for advanced prediction
        self.advancedPredictionFrame = Frame(self.master)
        t = Frame(self.advancedPredictionFrame)
        t.grid(row=0, column=0)
        # Production
        self.advanced_prediction_widgets = {}
        for i in range(10):
            self.advanced_prediction_widgets[str(i)] = Label(t, text=str(i) + "(x%)", font='arial 30')
            self.advanced_prediction_widgets[str(i)].grid(row=i, column=0)
        self.advanced_prediction_widgets["production"] = Label(self.advancedPredictionFrame, text="0", font='arial 300')
        self.advanced_prediction_widgets["production"].grid(row=0, column=1, padx=(30, 0))

        # Menu
        menu = Menu(self.master)
        self.master.config(menu=menu)
        option_menu = Menu(menu, tearoff=0)
        menu.add_cascade(label='Options', menu=option_menu)
        option_menu.add_command(label='Change View', command=self.change_view)
        option_menu.add_command(label='Clear Canvas', command=self.clear)
        option_menu.add_command(label='Exit', command=self.master.destroy)
        option_menu.add_command(label='Preview Input Img', command=self.preview_input_img)


if __name__ == '__main__':
    root = Tk()
    Main(root)
    root.title('HWDR Tester')
    root.mainloop()
