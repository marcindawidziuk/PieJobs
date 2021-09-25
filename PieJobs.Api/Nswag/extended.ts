export class ClientBase {
    protected transformOptions = (options: AxiosRequestConfig): Promise<AxiosRequestConfig> => {
        options.headers = {
            ...options.headers,
            'X-TOKEN': 'SomeToken',
        };
        return Promise.resolve(options);
    };
  }