
export class DirectLineToken {
    constructor(
        public value: DirectLineTokenValue,
        public hasError: boolean,
        public errors: any,
    ) { }
}

export class DirectLineTokenValue {
    constructor(
        public conversationId: any,
        public token: any,
        public expiresIn: any,
    ) { }
}
