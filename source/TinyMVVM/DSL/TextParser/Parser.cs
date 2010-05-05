﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TinyMVVM.SemanticModel.MVVM;

namespace TinyMVVM.DSL.TextParser
{
    public class Parser
    {
        private ILexicalAnalyzer scanner;
        private ModelSpecification modelSpecification;
        private ViewModel semanticModel;
        private TokenEnumerator tokensEnumerator;
        private string loadedCode;

        public Parser() : 
            this(new Scanner())
        {
            
        }

        public Parser(ILexicalAnalyzer scanner)
        {
            if (scanner == null)
                throw new ArgumentNullException("scanner");

            this.scanner = scanner;
        }

        public ModelSpecification Parse(ICodeLoader loadingStrategy)
        {
            if (loadingStrategy == null)
                throw new ArgumentNullException();

            loadedCode = loadingStrategy.Load();

            if (loadedCode == null)
                throw new InvalidOperationException("Code must be loaded before it's possible to parse");

            tokensEnumerator = new TokenEnumerator(scanner.Scan(loadedCode));

            modelSpecification = new ModelSpecification();

            ParseViewModels();

            return modelSpecification;
        }

        private void ParseViewModels()
        {
            while (CurrentToken() != Token.EOF)
            {
                if (CurrentToken() == Token.Using)
                    ParseUsing();
                else if (CurrentToken() == Token.ViewModel)
                    ParseViewModel();
                else
                    NextToken();
            }
        }

        private Token NextToken()
        {
            tokensEnumerator.MoveNext();
            return tokensEnumerator.Current;
        }

        private void ParseUsing()
        {
            var nameToken = NextToken();
            if (nameToken.Kind != Kind.Name)
                throw new InvalidSyntaxException("Namespace must be specified when using the 'using' keyword");

            modelSpecification.Usings.Add(nameToken.Value);

            NextToken();
        }

        private void ParseViewModel()
        {
            var nameToken = NextToken();
            if (nameToken.Kind != Kind.Name)
                throw new InvalidSyntaxException("Name must be specified when using the 'viewmodel' keyword");

            ParseViewModelName();
            ParseViewModelParent();
            ParseViewModelBody();
        }

        private void ParseViewModelName()
        {
            var nameToken = CurrentToken();
            semanticModel = new ViewModel(nameToken.Value);
            modelSpecification.AddViewModel(semanticModel);
        }

        private void ParseViewModelParent()
        {
            var token = NextToken();
            if (token == Token.Extends)
            {
                var parentNameToken = NextToken();
                semanticModel.Parent = parentNameToken.Value;
            }
        }

        private void ParseViewModelBody()
        {
            while (CurrentToken() != Token.ViewModel &&
                CurrentToken() != Token.EOF)
            {
                var token = tokensEnumerator.Current;
                if (token == Token.Property ||
                    token == Token.OProperty ||
                    token.Kind == Kind.attribute)
                    ParseViewModelProperties();
                else if (token == Token.Command)
                    ParseViewModelCommand();

                NextToken();
            }
        }

        private Token CurrentToken()
        {
            return tokensEnumerator.Current;   
        }

        private void ParseViewModelProperties()
        {
            var attributes = new List<string>();
            while (CurrentToken().Kind == Kind.attribute)
            {
                attributes.Add(CurrentToken().Value);
                NextToken();
            }

            var propertyToken = CurrentToken();
            var nameToken = NextToken();
            var asToken = NextToken();
            var typeToken = NextToken();

            if (nameToken.Kind != Kind.Name)
                throw new InvalidSyntaxException("Name must be specified when using the 'property' keyword");

            if (asToken.Kind != Kind.AS ||
                typeToken.Kind != Kind.Name)
                throw new InvalidSyntaxException("Type must be specified when using the 'property' keyword");

            //Name
            var name = nameToken.Value;

            //Type
            var type = typeToken.Value;

            //IsObservable
            var isObservable = (propertyToken == Token.OProperty) ? true : false;

            var viewModelProperty = new ViewModelProperty(name, type, isObservable);
            viewModelProperty.Attributes.AddRange(attributes);
            
            semanticModel.AddProperty(viewModelProperty);
        }

        private void ParseViewModelCommand()
        {
            var nameToken = NextToken();

            if (nameToken.Kind != Kind.Name)
                throw new InvalidSyntaxException("Name must be specified when using the 'command' keyword");

            semanticModel.AddCommand(
                new ViewModelCommand(nameToken.Value));
        }

        private class TokenEnumerator : IEnumerator<Token>
        {
            private int index = 0;
            private IEnumerable<Token> tokens;

            public TokenEnumerator(IEnumerable<Token> tokens)
            {
                this.tokens = tokens;
            }

            public void Dispose()
            {
                tokens = null;  
            }

            public bool MoveNext()
            {
                if (index + 1 < tokens.Count())
                {
                    index++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                index = 0;
            }

            public Token Current
            {
                get { return tokens.ElementAt(index);} 
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
