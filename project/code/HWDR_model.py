from torch import Tensor
from torch.nn import functional
import torch.nn as nn


class MyConNet1(nn.Module):

    def __init__(self):
        super(MyConNet1, self).__init__()

        self.conv1 = nn.Conv2d(in_channels=1, out_channels=14, kernel_size=5, padding=2, stride=1)
        self.activ1 = nn.ReLU()
        self.drop1 = nn.Dropout(p=0.3)

        self.conv2 = nn.Conv2d(in_channels=14, out_channels=7, kernel_size=5, padding=2, stride=1)
        self.activ2 = nn.ReLU()
        self.drop2 = nn.Dropout(p=0.3)

        self.dense1 = nn.Linear(in_features=7 * 28 * 28, out_features=32)
        self.activ3 = nn.ReLU()
        self.drop3 = nn.Dropout(p=0.3)

        self.dense2 = nn.Linear(in_features=32, out_features=10)

    def forward(self, x: Tensor) -> Tensor:
        x = self.conv1(x)
        x = self.activ1(x)
        x = self.drop1(x)

        x = self.conv2(x)
        x = self.activ2(x)
        x = x.view(-1, 7 * 28 * 28)
        x = self.drop2(x)

        x = self.dense1(x)
        x = self.activ3(x)
        x = self.drop3(x)

        x = self.dense2(x)

        return x


class MyConNet2(nn.Module):

    def __init__(self):
        super(MyConNet2, self).__init__()

        self.conv1 = nn.Conv2d(in_channels=1, out_channels=14, kernel_size=5, padding=2, stride=1)
        self.activ1 = nn.ReLU()
        self.drop1 = nn.Dropout(p=0.3)

        self.conv2 = nn.Conv2d(in_channels=14, out_channels=28, kernel_size=5, padding=2, stride=2)
        self.activ2 = nn.ReLU()
        self.drop2 = nn.Dropout(p=0.3)

        self.conv3 = nn.Conv2d(in_channels=28, out_channels=56, kernel_size=5, padding=2, stride=1)
        self.activ3 = nn.ReLU()
        self.drop3 = nn.Dropout(p=0.3)

        self.dense1 = nn.Linear(in_features=56 * 14 * 14, out_features=406)
        self.activ4 = nn.ReLU()
        self.drop4 = nn.Dropout(p=0.3)

        self.dense2 = nn.Linear(in_features=406, out_features=32)
        self.activ5 = nn.ReLU()
        self.drop5 = nn.Dropout(p=0.3)

        self.dense3 = nn.Linear(in_features=32, out_features=10)

    def forward(self, x: Tensor) -> Tensor:
        x = self.conv1(x)
        x = self.activ1(x)
        x = self.drop1(x)

        x = self.conv2(x)
        x = self.activ2(x)
        x = self.drop2(x)

        x = self.conv3(x)
        x = self.activ3(x)
        x = x.view(-1, 56 * 14 * 14)
        x = self.drop3(x)

        x = self.dense1(x)
        x = self.activ4(x)
        x = self.drop4(x)

        x = self.dense2(x)
        x = self.activ5(x)
        x = self.drop5(x)

        x = self.dense3(x)

        return x


class MyConNet3(nn.Module):

    def __init__(self):
        super(MyConNet3, self).__init__()

        self.conv1 = nn.Conv2d(in_channels=1, out_channels=8, kernel_size=5, stride=1)
        self.activ1 = nn.ReLU()
        self.drop1 = nn.Dropout(p=0.3)

        self.conv2 = nn.Conv2d(in_channels=8, out_channels=16, kernel_size=5, stride=1)
        self.activ2 = nn.ReLU()
        self.drop2 = nn.Dropout(p=0.3)

        self.conv3 = nn.Conv2d(in_channels=16, out_channels=32, kernel_size=5, stride=1)
        self.activ3 = nn.ReLU()
        self.drop3 = nn.Dropout(p=0.3)

        self.dense1 = nn.Linear(in_features=32 * 16 * 16, out_features=560)
        self.activ4 = nn.Tanh()
        self.drop4 = nn.Dropout(p=0.3)

        self.dense3 = nn.Linear(in_features=560, out_features=10)

    def forward(self, x: Tensor) -> Tensor:
        x = self.conv1(x)
        x = self.activ1(x)
        x = self.drop1(x)

        x = self.conv2(x)
        x = self.activ2(x)
        x = self.drop2(x)

        x = self.conv3(x)
        x = self.activ3(x)
        x = x.view(-1, 32 * 16 * 16)
        x = self.drop3(x)

        x = self.dense1(x)
        x = self.activ4(x)
        x = self.drop4(x)

        x = self.dense3(x)

        return x


class LeNet5Variant(nn.Module):
    def __init__(self):
        super(LeNet5Variant, self).__init__()
        self.conv1 = nn.Conv2d(1, 6, 5)
        self.pool = nn.MaxPool2d(2, 2)
        self.conv2 = nn.Conv2d(6, 16, 5)
        self.fc1 = nn.Linear(16 * 4 * 4, 120)
        self.fc2 = nn.Linear(120, 84)
        self.fc3 = nn.Linear(84, 10)

    def forward(self, x):
        x = self.pool(functional.relu(self.conv1(x)))
        x = self.pool(functional.relu(self.conv2(x)))
        x = x.view(-1, 16 * 4 * 4)
        x = functional.relu(self.fc1(x))
        x = functional.relu(self.fc2(x))
        x = self.fc3(x)
        return x
