from HWDR_model import MyConNet1, MyConNet2
import torch
import torchvision.transforms as transforms
import torch.nn as nn
from torch import Tensor

# Defines transformations for dataset normalization
img_transforms = transforms.Compose([
    transforms.ToTensor(),
    transforms.Normalize((0.1305,), (0.3081,))
])

# Data classes
data_classes = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9']
torch.set_printoptions(threshold=1)


class Classifier:

    def __init__(self):
        self.model = MyConNet2()
        self.model.load_state_dict(
            torch.load("C:\\Users\\kubaa\\Documents\\GitHub\\Python\\HWDR-PreTrained\\modelMyConNet2.pt"))
        self.model.eval()

    def classify(self, img):
        img = img_transforms(img).float()
        self.model.eval()
        output = self.model(img.reshape(1, 1, 28, 28))
        output = nn.Softmax(dim=1)(output)
        _, predicted = torch.max(output.data, 1)
        return data_classes[predicted.item()], output.detach().numpy()[0]
