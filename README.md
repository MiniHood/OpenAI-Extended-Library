# OpenAI-Extended-Library
This is a C# library for interacting with the OpenAI API. The library provides a simple and easy-to-use interface for a wide range of AI tasks, including:

# PLEASE READ

This is a open source old version, it doesn't have the features that my paid version does.

The paid version is available in releases with a trial period. It is NOT for commerical use and shall not be used as such. You can use in for personal use but the pop up will be annoying. Please note if you'd like to purchase the full version it will be available on my website.

## Getting Started

To start using the OpenAI C# Library, you'll need an API key from OpenAI. You can sign up for an API key [here](https://beta.openai.com/signup/).

You can download the library from releases.

## Usage

To use the library, first create an instance of the `Client` class and provide your API key:
```csharp
var apiKey = "your-api-key";
var endpoint = "https://api.openai.com/v1/";
var client = new Client(apiKey, endpoint);
```

You can now use the Client instance to call the various methods available in the library. For example, to generate text, you would call the GenerateText method:

```csharp
var model = "text-davinci-002";
var prompt = "The future is bright.";
var length = 100;
var nopunct = false;
var stop = false;
var temperature = 0.5;
var result = client.GenerateText(model, prompt, length, nopunct, stop, temperature);
```

The library also provides methods for summarizing text, completing code, analyzing sentiment, recognizing named entities, classifying text, translating text, and recognizing emotions in text or speech.

For more information on how to use each method, see the API reference section below.

## API Reference
# GenerateText
Generates text using the OpenAI API.

```csharp
public string GenerateText(string model, string prompt, int length, bool nopunct, bool stop, int temperature)
```

Parameters
`model`: The name of the OpenAI model to use for text generation.

`prompt`: The text prompt to use as input to the model.

`length`: The maximum length of the generated text, in characters.

`nopunct`: A flag indicating whether to include punctuation in the generated text.

`stop`: A flag indicating whether to stop generating text when the API returns an end-of-text token.

`temperature`: The temperature to use for text generation.


Returns
The generated text.

# SummarizeText
Summarizes text using the OpenAI API.

```csharp
public string SummarizeText(string text, int length)
```

Parameters
`text`: The text to summarize.

`length`: The desired length of the summarized text, in characters.

Returns
The summarized text.

#CompleteCode
Completes code using the OpenAI API.
```csharp
public string CompleteCode(string model, string prompt, int maxTokens, int n)
```

Parameters
`model`: The name of the OpenAI model to use for code completion.

`prompt`: The code prompt to use as input to the model.

`maxTokens`: The maximum number of tokens to generate in the completed code.

`n`: The number of completions to generate for the prompt.

Returns
The completed code.

#AnalyzeSentiment
Analyzes the sentiment of text using the OpenAI API.

```csharp
public string AnalyzeSentiment(string text)
```

Parameters
`text`: The text to analyze.

Returns
The sentiment of the text, as a string value representing positive, negative, or neutral sentiment.


#RecognizeEntities
Recognizes named entities in text using the OpenAI API.

```csharp
public string RecognizeEntities(string text)
```

Parameters
`text`: The text to recognize entities in.

Returns
A list of recognized entities and their types.

#ClassifyText
Classifies text into categories using the OpenAI API.

```csharp
public string ClassifyText(string text)
```

Parameters
`text`: The text to classify.

Returns
A list of categories that the text belongs to.

#TranslateText
Translates text from one language to another using the OpenAI API.

```csharp
public string TranslateText(string text, string from, string to)
```

Parameters
`text`: The text to translate.

`from`: The source language of the text.

`to`: The target language to translate the text into.

Returns
The translated text.

#RecognizeEmotion
Recognizes emotions in text or speech (Coming Soon) using the OpenAI API.

```csharp
public string RecognizeEmotion(string text)
```

Parameters
`text`: The text or speech to recognize emotions in.

Returns
A list of emotions detected in the text or speech, along with the confidence level for each emotion.

## Todo
Generating image captions
Synthesizing speech
Secognizing speech
