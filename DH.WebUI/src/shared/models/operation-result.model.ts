export interface OperationResult<T = undefined> {
    validationErrors: { [key: string]: string[] };
    success: boolean;
    initialException?: IError;
    relatedObject?: T;
}

export interface IError {
    type: string;
    code: number;
    message: string;
}