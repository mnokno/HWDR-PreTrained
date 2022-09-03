import random
from tkinter import *
from tkinter import ttk
from PIL import ImageGrab
from PIL import Image

from helper import Classifier

class main:
    def __init__(self, master):
        self.classifier = Classifier()
        self.master = master
        self.color_fg = 'black'
        self.color_bg = 'white'
        self.old_x = None
        self.old_y = None
        self.pen_width = 5
        self.drawWidgets()
        self.c.bind('<B1-Motion>',self.paint)#drwaing the line
        self.c.bind('<ButtonRelease-1>',self.reset)
        self.detailed_prediction_report = False

    def GetImg(self):
        x = root.winfo_rootx() + self.c.winfo_x()
        y = root.winfo_rooty() + self.c.winfo_y()
        x1 = x + self.c.winfo_width()
        y1 = y + self.c.winfo_height()
        img = ImageGrab.grab().crop((x, y, x1, y1))
        img = img.convert('1')
        img = img.resize((28, 28), reducing_gap=4)
        #img.save("C:\\Users\\kubaa\\Documents\\GitHub\\Python\\HWDR-PreTrained\\digit.png")
        return img

    def previewInputImg(self):
        self.GetImg().show()

    def paint(self, e):
        if self.old_x and self.old_y:
            self.c.create_line(self.old_x, self.old_y, e.x, e.y, width=self.pen_width, fill="black", capstyle=ROUND, smooth=True)

        self.old_x = e.x
        self.old_y = e.y
        img_class, data = self.classifier.classify(self.GetImg())
        print(img_class)
        self.prodictionLabel.config(text=img_class)
        self.advancedPredictionWigets["production"].config(text=img_class)
        for i in range(10):
            if data[i] > 0.1:
                self.advancedPredictionWigets[str(i)].config(
                    text=str((i + 1) % 10) + "({:0.1f}%)".format(data[i] * 100))
            else:
                self.advancedPredictionWigets[str(i)].config(
                    text=str((i + 1) % 10) + "({:0.1f}0%)".format(data[i] * 100))

    # resting or cleaning the canvas
    def reset(self, e):
        self.old_x = None
        self.old_y = None

    # change Width of pen through slider
    def changeW(self, e):
        self.pen_width = e

    # clears the canvas
    def clear(self):
        self.c.delete(ALL)

    # changes view between detailed and simple
    def change_view(self):
        self.detailed_prediction_report = not self.detailed_prediction_report

        if self.detailed_prediction_report:
            self.simplePredictionFrame.grid_remove()
            self.advancedPredictionFrame.grid(row=0, column=1)
        else:
            self.simplePredictionFrame.grid(row=0, column=1)
            self.advancedPredictionFrame.grid_remove()

    def drawWidgets(self):

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
        self.simplePredictionFrame = Frame(self.master)
        self.simplePredictionFrame.grid(row=0, column=1)
        # Production
        self.prodictionLabel = Label(self.simplePredictionFrame, text="0", font='arial 300')
        self.prodictionLabel.grid(row=0, column=0, sticky="nw")
        self.confidanceLabel = Label(self.simplePredictionFrame, text="0%", font='arial 50')
        self.confidanceLabel.grid(row=1, column=0)

        # Frame for advanced prediction
        self.advancedPredictionFrame = Frame(self.master)
        #self.advancedPredictionFrame.grid(row=0, column=1)
        t = Frame(self.advancedPredictionFrame)
        t.grid(row=0, column=0)
        # Production
        self.advancedPredictionWigets = {}
        for i in range(10):
            self.advancedPredictionWigets[str(i)] = Label(t, text=str(i) + "(x%)", font='arial 30')
            self.advancedPredictionWigets[str(i)].grid(row=i, column=0)
        self.advancedPredictionWigets["production"] = Label(self.advancedPredictionFrame, text="0", font='arial 300')
        self.advancedPredictionWigets["production"].grid(row=0, column=1, padx=(30, 0))

        # Menu
        menu = Menu(self.master)
        self.master.config(menu=menu)
        optionmenu = Menu(menu, tearoff=0)
        menu.add_cascade(label='Options', menu=optionmenu)
        optionmenu.add_command(label='Change View', command=self.change_view)
        optionmenu.add_command(label='Clear Canvas', command=self.clear)
        optionmenu.add_command(label='Exit', command=self.master.destroy)
        optionmenu.add_command(label='Preview Input Img', command=self.previewInputImg)

if __name__ == '__main__':
    root = Tk()
    main(root)
    root.title('HWDR Tester')
    root.mainloop()