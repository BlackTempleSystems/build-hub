export class BaseServerResponse<ResponseData> {
    dateTimeStamp?: Date;
    isSuccessful?: boolean;
    responseData?: ResponseData;
}