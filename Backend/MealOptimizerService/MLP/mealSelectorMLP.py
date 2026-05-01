import torch.nn as nn

class MealSelectorMLP(nn.Module):
    def __init__(self, num_combinations):
        super(MealSelectorMLP, self).__init__()
        # Input size is 3: [Norm_Cal, Norm_Prot, Norm_Meal_ID]
        self.network = nn.Sequential(
            nn.Linear(3, 512),
            nn.ReLU(),
            nn.Linear(512, 256),
            nn.ReLU(),
            nn.Linear(256, num_combinations)
        )

    def forward(self, x):
        return self.network(x)