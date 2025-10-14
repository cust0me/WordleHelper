using WordleHelper.Core.Services;

char[] blacklist = [

];

char[][] wrongPositions = [
    [],
    [],
    [],
    [],
    [],
];

char[] mustInclude = [
    
];

char[] known = [
    ' ',
    ' ',
    ' ',
    ' ',
    ' '
];

WordStorageService wordStorageService = new();
WordlePredictionService wordlePredictionService = new(wordStorageService);

List<string> predictions = await wordlePredictionService.GetPredictions(known, wrongPositions, mustInclude, blacklist);
foreach (string s in predictions)
{
    Console.WriteLine(s);
}