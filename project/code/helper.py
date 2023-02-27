import numpy
import torch
import torchvision.transforms as transforms
import torch.nn as nn
from HWDR_model import MyConNet1, MyConNet2, MyConNet3, LeNet5Variant, MyConNet6

import PIL
from PIL import Image, ImageFilter, ImageOps

# Defines transformations for dataset normalization
# Repeated from the train.py
img_transforms = transforms.Compose([
    transforms.ToTensor(),
    transforms.Normalize((0.1305,), (0.3081,))
])

# Classes to label, it this case the class is the label e.g 0 => '0'
data_classes = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9']
# Sets precision for tensors, automatically round confidence intervals when classifying
torch.set_printoptions(threshold=1)


# Classifier class is used to classy images using pre-trained CNN
class Classifier:

    def __init__(self):
        self.model = MyConNet6()
        self.model.load_state_dict(torch.load("../saved_models/MyConNet6.pt"))
        self.model.eval()

    # Classifies given image using preloaded CNN, returns image class and prediction matrix as a numpy array
    # NOTE: we assume that the image we receive is correctly edited e.g (white digit on black background)
    def classify(self, img) -> (str, numpy.array):
        # Applies image transformation and ensures that the image is represented using flats
        img = img_transforms(img).float()
        # Ensures that the model is in eval mode
        self.model.eval()
        # Feeds the image through the model
        output = self.model(img.reshape(1, 1, 28, 28))
        # Applies softmax to convert the output to represent percentages
        output = nn.Softmax(dim=1)(output)
        # Extracts the max production from the output, torch.max(...) => (max_value, value_index)
        _, predicted = torch.max(output.data, 1)
        # Returns the production and prediction matrix as a numpy array
        return data_classes[predicted.item()], output.detach().numpy()[0]

    @staticmethod
    # Formats a standard image of a digit for classification
    def format_image(img: Image, invert_colors: bool) -> Image:

        img = img.resize((28, 28))  # Resizes the image to match models input
        img = img.convert('L')  # Converts the image to grayscale (one channel)
        img = img.filter(ImageFilter.GaussianBlur(0.8))  # Applies blur to reduce pixelation from downscaling
        if invert_colors:  # Invert colors is required, model expects white digit on black background
            img = PIL.ImageOps.invert(img)

        return img
